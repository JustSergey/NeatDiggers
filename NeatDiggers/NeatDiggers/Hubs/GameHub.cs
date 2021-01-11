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
            Room room = Server.AddUser(null, Context.ConnectionId, code, true);
            if (room != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, code);
                await Clients.Group(code).ChangeState(room);
            }
        }

        public async Task<string> ConnectToRoom(string code, string name)
        {
            Room room = Server.AddUser(name, Context.ConnectionId, code, false);
            if (room != null && !room.IsStarted)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, code);
                await Clients.Group(code).ChangeState(room);
                return Context.ConnectionId;
            }

            return null;
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
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null)
                    return room.GetGameMap();
            }
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
                    List<Item> newItems = inventory.Items;
                    int hands = 0;
                    WeaponType weaponType = WeaponType.None;
                    if (inventory.LeftWeapon != null && inventory.LeftWeapon.Type == ItemType.Weapon)
                    {
                        newItems.Add(inventory.LeftWeapon);
                        hands += (int) inventory.LeftWeapon.WeaponHanded;
                        weaponType = inventory.LeftWeapon.WeaponType;
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

                    if (inventory.Armor != null && inventory.Armor.Type == ItemType.Armor)
                    {
                        newItems.Add(inventory.Armor);
                    }

                    if (hands > player.Hands)
                        return false;

                    List<Item> oldItems = new List<Item>(player.Inventory.Items)
                    {
                        player.Inventory.LeftWeapon,
                        player.Inventory.RightWeapon
                    };

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
            if (Context.Items.ContainsKey("IsDice") && (bool) Context.Items["IsDice"])
                return (int) Context.Items["Dice"];
            Random random = new Random();
            int dice = random.Next(1, 7);
            Context.Items["IsDice"] = true;
            Context.Items["Dice"] = dice;
            return dice;
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
                    if (actionsCount == 0 || (actionsCount == 1 && prevAction != gameAction.Type))
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
                            Context.Items["IsDice"] = false;
                            await Clients.Group(room.Code).ChangeStateWithAction(room, gameAction);
                            return true;
                        }
                        Context.Items["Actions"] = actionsCount + 1;
                        Context.Items["PrevAction"] = gameAction.Type;
                    }
                }
            }
            return false;
        }

        private bool Move(Room room, GameAction gameAction)
        {
            if (Context.Items.TryGetValue("Dice", out object _dice) && _dice is int dice)
            {
                Vector playerPosition = gameAction.CurrentPlayer.Position;
                Vector targetPosition = gameAction.TargetPosition;
                int x = targetPosition.X;
                int y = targetPosition.Y;
                if (playerPosition.CheckAvailability(targetPosition, diceRollResult) &&
                    targetPosition.IsInMap(room.GameMap) &&
                    room.GameMap.Map[x, y] != Cell.None && room.GameMap.Map[x, y] != Cell.Wall)
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
            Vector targetPosition = gameAction.TargetPosition;

            int playerAttackRadius = CalculateAttackRadius(gameAction.CurrentPlayer);
            int playerAttackDamage = GetPlayerDamage(gameAction.CurrentPlayer);

            Item enemyArmor = gameAction.TargetPlayer.Inventory.Armor;
            int enemyArmorBuff = gameAction.TargetPlayer.Armor;
            int enemyArmorStrength = enemyArmor.ArmorStrength;

            if (playerPosition.CheckAvailability(targetPosition, playerAttackRadius))
            {
                room.GetPlayer(gameAction.TargetPlayer.Id).Health -=
                    playerAttackDamage - enemyArmorStrength - enemyArmorBuff;

                if (enemyArmor.Type == ItemType.Armor)
                {
                    enemyArmor.ArmorDurability--;
                    if (enemyArmor.ArmorDurability <= 0)
                        room.GetPlayer(gameAction.TargetPlayer.Id).Inventory.Armor = new EmptyItem();
                }

                if (room.GetPlayer(gameAction.TargetPlayer.Id).Health <= 0)
                {
                    KillPlayer(room, gameAction.TargetPlayer);
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool Dig(Room room, GameAction gameAction)
        {
            if (Context.Items.TryGetValue("Dice", out object _dice) && _dice is int dice)
            {
                if (dice % 2 == 0)
                {
                    int count = dice / 2 + gameAction.CurrentPlayer.DigPower;
                    for (int i = 0; i < count; i++)
                    {
                        Item item = room.Dig();
                        if (item.Type == ItemType.Event)
                        {
                            item.Use(room, gameAction);
                            continue;
                        }
                        else if (item.Type == ItemType.Passive)
                            item.Get(room, gameAction);

                        room.GetPlayer(gameAction.CurrentPlayer.Id).Inventory.Items.Add(item);
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
                item.Use(room, gameAction);
                gameAction.CurrentPlayer.Inventory.Items.Remove(item);
                return true;
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
            {
                gameAction.Ability.Use(room, gameAction);
                return true;
            }
            return false;
        }

        private void KillPlayer(Room room, Player player)
        {
            room.GetPlayer(player.Id).Health = player.Character.MaxHealth;
            room.GetPlayer(player.Id).Inventory = new Inventory();
            room.GetPlayer(player.Id).Position = player.SpawnPoint;
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
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Code);
                await Clients.Group(room.Code).ChangeState(room);
            }

            await base.OnDisconnectedAsync(exception);
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