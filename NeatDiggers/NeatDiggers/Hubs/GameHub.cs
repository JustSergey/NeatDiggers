using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer;
using NeatDiggers.GameServer.Characters;
using NeatDiggers.GameServer.Items;
using Microsoft.AspNetCore.Authorization;
using NeatDiggers.GameServer.Abilities;
using NeatDiggers.GameServer.Maps;

namespace NeatDiggers.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public async Task ConnectToRoomAsSpectator(string code)
        {
            Room room = Server.GetRoom(code);
            if (room != null)
            {
                if (Server.AddUser(Context.ConnectionId, code) && room.AddSpectator(Context.ConnectionId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, code);
                    await Clients.Group(code).ChangeState(room);
                }
            }
        }

        public async Task<string> ConnectToRoom(string code, string name)
        {
            Room room = Server.GetRoom(code);
            if (room != null)
            {
                if (!room.IsStarted)
                {
                    if (Server.AddUser(Context.ConnectionId, code) && room.AddPlayer(Context.ConnectionId, name))
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, code);
                        await Clients.Group(code).ChangeState(room);
                        return Context.ConnectionId;
                    }
                    return "full";
                }
                return "started";
            }
            return "wrongCode";
        }

        public async Task ChangeCharacter(CharacterName characterName)
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && !player.IsReady)
                {
                    player.ChangeCharacter(characterName);
                    await Clients.Group(room.Code).ChangeState(room);
                }
            }
        }

        public async Task ChangeReady()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null)
                {
                    player.ChangeReady();
                    await Clients.Group(room.Code).ChangeState(room);
                }
            }
            Context.Items["Actions"] = 0;
            Context.Items["PrevAction"] = GameActionType.DropItem;
        }

        public async Task StartGame()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && room.Start())
                    await Clients.Group(room.Code).ChangeState(room);
            }
        }

        public GameMap GetGameMap()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null)
                return room.GetGameMap();
            return null;
        }

        public async Task<bool> ChangeInventory(Inventory inventory)
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && player.IsTurn)
                {
                    List<Item> newItems = inventory.Items.Where(i => i.Name != ItemName.Empty).ToList();
                    int hands = 0;
                    WeaponType weaponType = player.Character.WeaponType;
                    if (inventory.LeftWeapon != null && inventory.LeftWeapon.Type == ItemType.Weapon)
                    {
                        if (weaponType == WeaponType.None || inventory.LeftWeapon.WeaponType == weaponType)
                        {
                            newItems.Add(inventory.LeftWeapon);
                            hands += (int)inventory.LeftWeapon.WeaponHanded;
                        }
                        else
                            return false;
                    }

                    if (inventory.RightWeapon != null && inventory.RightWeapon.Type == ItemType.Weapon)
                    {
                        if (weaponType == WeaponType.None || inventory.RightWeapon.WeaponType == weaponType)
                        {
                            newItems.Add(inventory.RightWeapon);
                            hands += (int) inventory.RightWeapon.WeaponHanded;
                        }
                        else
                            return false;
                    }

                    if (hands > player.Hands)
                        return false;

                    if (inventory.Armor != null && inventory.Armor.Type == ItemType.Armor)
                        newItems.Add(inventory.Armor);

                    List<Item> oldItems = new List<Item>(player.Inventory.Items)
                    {
                        player.Inventory.LeftWeapon,
                        player.Inventory.RightWeapon,
                        player.Inventory.Armor
                    }.Where(i => i.Name != ItemName.Empty).ToList();

                    if (oldItems.Count != newItems.Count)
                        return false;

                    newItems.Sort((item1, item2) => item2.Name - item1.Name);
                    oldItems.Sort((item1, item2) => item2.Name - item1.Name);

                    for (int i = 0; i < newItems.Count; i++)
                    {
                        if (newItems[i].Name != oldItems[i].Name)
                            return false;
                    }

                    player.Inventory = inventory;
                    await Clients.Group(room.Code).ChangeState(room);
                    return true;
                }
            }
            return false;
        }

        public int RollTheDice()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && player.IsTurn)
                {
                    if (Context.Items.ContainsKey("IsDice") && (bool)Context.Items["IsDice"])
                        return (int)Context.Items["Dice"];
                    Random random = new Random();
                    int dice = random.Next(1, 7);
                    Context.Items["IsDice"] = true;
                    Context.Items["Dice"] = dice;
                    return dice;
                }
            }
            return -1;
        }

        public async Task<bool> DoAction(GameAction gameAction)
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player.IsTurn)
                {
                    int actionsCount = (int)Context.Items["Actions"];
                    GameActionType prevAction = (GameActionType)Context.Items["PrevAction"];
                    if (actionsCount == 0 || 
                        (actionsCount == 1 && prevAction != gameAction.Type) || 
                        gameAction.Type == GameActionType.DropItem)
                    {
                        gameAction.CurrentPlayer = player;
                        Func<bool> action = gameAction.Type switch
                        {
                            GameActionType.Move => () => Move(room, gameAction),
                            GameActionType.Dig => () => Dig(room, gameAction),
                            GameActionType.Attack => () => Attack(room, gameAction),
                            GameActionType.UseItem => () => UseItem(room, gameAction),
                            GameActionType.DropItem => () => DropItem(room, gameAction),
                            GameActionType.UseAbility => () => UseAbility(room, gameAction),
                            _ => null
                        };
                        if (action != null && action())
                        {
                            if (gameAction.Type == GameActionType.Move || gameAction.Type == GameActionType.Dig)
                                Context.Items["IsDice"] = false;
                            if (gameAction.Type != GameActionType.DropItem)
                            {
                                Context.Items["Actions"] = actionsCount + 1;
                                Context.Items["PrevAction"] = gameAction.Type;
                            }
                            await Clients.Group(room.Code).ChangeStateWithAction(room, gameAction);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool Move(Room room, GameAction gameAction)
        {
            if (Context.Items.TryGetValue("Dice", out object _dice) && _dice is int dice && (bool)Context.Items["IsDice"])
            {
                Vector playerPosition = gameAction.CurrentPlayer.Position;
                Vector targetPosition = gameAction.TargetPosition;
                int x = targetPosition.X;
                int y = targetPosition.Y;
                if (playerPosition.CheckAvailability(targetPosition, dice + gameAction.CurrentPlayer.Speed) &&
                    targetPosition.IsInMap(room.GetGameMap()) &&
                    room.GetGameMap().Map[x, y] != Cell.None && room.GetGameMap().Map[x, y] != Cell.Wall)
                {
                    room.GetPlayer(gameAction.CurrentPlayer.Id).Position = targetPosition;
                    return true;
                }
            }
            return false;
        }

        private bool Attack(Room room, GameAction gameAction)
        {
            Vector playerPosition = gameAction.CurrentPlayer.Position;
            Player targetPlayer = room.GetPlayer(gameAction.TargetPlayerId);

            int playerAttackRadius = CalculateAttackRadius(gameAction.CurrentPlayer);
            int playerAttackDamage = GetPlayerDamage(gameAction.CurrentPlayer);

            Item enemyArmor = targetPlayer.Inventory.Armor;
            int enemyArmorBuff = targetPlayer.Armor;
            int enemyArmorStrength = enemyArmor.ArmorStrength;

            if (playerPosition.CheckAvailability(targetPlayer.Position, playerAttackRadius))
            {
                int consumption = 
                    gameAction.CurrentPlayer.Inventory.LeftWeapon.WeaponConsumption +
                    gameAction.CurrentPlayer.Inventory.RightWeapon.WeaponConsumption;

                if (gameAction.CurrentPlayer.Inventory.Drop >= consumption)
                {
                    gameAction.CurrentPlayer.Inventory.Drop -= consumption;

                    targetPlayer.Health -=
                        playerAttackDamage - enemyArmorStrength - enemyArmorBuff;

                    if (enemyArmor.Type == ItemType.Armor)
                    {
                        enemyArmor.ArmorDurability--;
                        if (enemyArmor.ArmorDurability <= 0)
                            targetPlayer.Inventory.Armor = new EmptyItem();
                    }

                    if (targetPlayer.Health <= 0)
                    {
                        gameAction.CurrentPlayer.LevelUp();
                        targetPlayer.Respawn();
                    }

                    return true;
                }
            }
            return false;
        }

        private bool Dig(Room room, GameAction gameAction)
        {
            if (Context.Items.TryGetValue("Dice", out object _dice) && _dice is int dice && (bool)Context.Items["IsDice"])
            {
                Vector playerPosition = gameAction.CurrentPlayer.Position;
                if (room.GetGameMap().Map[playerPosition.X, playerPosition.Y] == Cell.Digging && dice % 2 == 0)
                {
                    int count = dice / 2 + gameAction.CurrentPlayer.DigPower;
                    for (int i = 0; i < count; i++)
                    {
                        Item item = room.Dig();
                        if (item.Type == ItemType.Passive)
                            item.Get(room, gameAction);

                        gameAction.CurrentPlayer.Inventory.Items.Add(item);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool UseItem(Room room, GameAction gameAction)
        {
            Item item = gameAction.CurrentPlayer.Inventory.Items.Find(i => i.Name == gameAction.Item.Name);
            if (item != null && item.Type == ItemType.Active)
            {
                if (item.Use(room, gameAction))
                {
                    gameAction.CurrentPlayer.Inventory.Items.Remove(item);
                    return true;
                }
            }
            return false;
        }

        private bool DropItem(Room room, GameAction gameAction)
        {
            return gameAction.CurrentPlayer.Inventory.DropItem(gameAction.Item, room, gameAction);
        }

        private bool UseAbility(Room room, GameAction gameAction)
        {
            Ability ability = gameAction.CurrentPlayer.Character.Abilities.Find(a => a.Name == gameAction.Ability.Name);
            if (ability != null && ability.Type == AbilityType.Active && ability.IsActive)
                return gameAction.Ability.Use(room, gameAction);
            return false;
        }

        public async Task<bool> EndTurn()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && player.IsTurn && player.Inventory.Items.Count <= Inventory.MaxItems)
                {
                    Context.Items["IsDice"] = false;
                    Context.Items["Actions"] = 0;
                    room.NextTurn();
                    await Clients.Group(room.Code).ChangeState(room);
                    return true;
                }
            }
            return false;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null)
            {
                room.Disconnect(Context.ConnectionId);
                Server.RemoveUser(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Code);
                if (!Server.RemoveEmptyRoom(room))
                    await Clients.Group(room.Code).ChangeState(room);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public int GetAttackRadius()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && player.IsTurn)
                    return CalculateAttackRadius(player);
            }
            return -1;
        }

        private int CalculateAttackRadius(Player player)
        {
            Item leftWeapon = player.Inventory.LeftWeapon;
            Item rightWeapon = player.Inventory.RightWeapon;

            if (leftWeapon.WeaponHanded == WeaponHanded.Two)
            {
                if (leftWeapon.WeaponType == WeaponType.Melee)
                {
                    return leftWeapon.WeaponDistance + player.MeleeDistance;
                }
                else if (leftWeapon.WeaponType == WeaponType.Ranged)
                {
                    return leftWeapon.WeaponDistance + player.RangedDistance;
                }
            }

            if (leftWeapon.WeaponType != WeaponType.None && rightWeapon.WeaponType != WeaponType.None)
            {
                if (leftWeapon.WeaponType == WeaponType.Melee)
                {
                    return Math.Min(leftWeapon.WeaponDistance, rightWeapon.WeaponDistance) + player.MeleeDistance;
                }
                else if (leftWeapon.WeaponType == WeaponType.Ranged)
                {
                    return Math.Min(leftWeapon.WeaponDistance, rightWeapon.WeaponDistance) + player.RangedDistance;
                }
            }

            if (leftWeapon.WeaponHanded == WeaponHanded.One)
            {
                if (leftWeapon.WeaponType == WeaponType.Melee)
                {
                    return leftWeapon.WeaponDistance + player.MeleeDistance;
                }
                else if (leftWeapon.WeaponType == WeaponType.Ranged)
                {
                    return leftWeapon.WeaponDistance + player.RangedDistance;
                }
            }
            else if (rightWeapon.WeaponHanded == WeaponHanded.One)
            {
                if (rightWeapon.WeaponType == WeaponType.Melee)
                {
                    return rightWeapon.WeaponDistance + player.MeleeDistance;
                }
                else if (rightWeapon.WeaponType == WeaponType.Ranged)
                {
                    return rightWeapon.WeaponDistance + player.RangedDistance;
                }
            }

            return player.MeleeDistance;
        }

        private int GetPlayerDamage(Player player)
        {
            int handsAttackDamage = 1;

            Item leftWeapon = player.Inventory.LeftWeapon;
            Item rightWeapon = player.Inventory.RightWeapon;

            if (leftWeapon.WeaponHanded == WeaponHanded.Two)
            {
                if (leftWeapon.WeaponType == WeaponType.Melee)
                {
                    return CalculateAttackDamage(leftWeapon.WeaponDamage, player.MeleeDamage, player.MultiplyDamage);
                }
                else if (leftWeapon.WeaponType == WeaponType.Ranged)
                {
                    return CalculateAttackDamage(leftWeapon.WeaponDamage, player.RangedDamage, player.MultiplyDamage);
                }
            }

            if (leftWeapon.WeaponType != WeaponType.None && rightWeapon.WeaponType != WeaponType.None)
            {
                if (leftWeapon.WeaponType == WeaponType.Melee)
                {
                    return CalculateAttackDamage(leftWeapon.WeaponDamage + rightWeapon.WeaponDamage, player.MeleeDamage,
                        player.MultiplyDamage);
                }
                else if (leftWeapon.WeaponType == WeaponType.Ranged)
                {
                    return CalculateAttackDamage(leftWeapon.WeaponDamage + rightWeapon.WeaponDamage,
                        player.RangedDamage, player.MultiplyDamage);
                }
            }

            if (leftWeapon.WeaponHanded == WeaponHanded.One)
            {
                if (leftWeapon.WeaponType == WeaponType.Melee)
                {
                    return CalculateAttackDamage(leftWeapon.WeaponDamage, player.MeleeDamage, player.MultiplyDamage);
                }
                else if (leftWeapon.WeaponType == WeaponType.Ranged)
                {
                    return CalculateAttackDamage(leftWeapon.WeaponDamage, player.RangedDamage, player.MultiplyDamage);
                }
            }
            else if (rightWeapon.WeaponHanded == WeaponHanded.One)
            {
                if (rightWeapon.WeaponType == WeaponType.Melee)
                {
                    return CalculateAttackDamage(rightWeapon.WeaponDamage, player.MeleeDamage, player.MultiplyDamage);
                }
                else if (rightWeapon.WeaponType == WeaponType.Ranged)
                {
                    return CalculateAttackDamage(rightWeapon.WeaponDamage, player.RangedDamage, player.MultiplyDamage);
                }
            }

            return CalculateAttackDamage(handsAttackDamage, player.MeleeDamage, player.MultiplyDamage);
        }

        private int CalculateAttackDamage(int weaponDamage, int damageBuff, double damageMultipluyer)
        {
            return (int) Math.Round((weaponDamage + damageBuff) * damageMultipluyer);
        }
    }
}