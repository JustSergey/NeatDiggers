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

namespace NeatDiggers.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public async Task<Room> ConnectToRoomAsSpectator(string code)
        {
            Room room = Server.GetRoom(code);
            if (room != null)
            {
                if (room.AddSpectator(Context.ConnectionId))
                {
                    Server.AddUser(Context.ConnectionId, new User { Code = code, Token = null });
                    await Groups.AddToGroupAsync(Context.ConnectionId, code);
                    await Clients.Group(code).ChangeState(room);
                    return room;
                }
            }
            return null;
        }

        public string GetUserId()
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user != null)
                return user.Id;
            return null;
        }

        public async Task<Room> ConnectToRoom(User user)
        {
            Context.Items["Actions"] = 0;
            Context.Items["PrevAction"] = GameActionType.DropItem;
            Room room = Server.GetRoom(user.Code);
            if (room != null && user.Token != null)
            {
                user.Id = user.GetId();
                if (room.Connect(user.Id, user.Name))
                {
                    Server.AddUser(Context.ConnectionId, user);
                    await Groups.AddToGroupAsync(Context.ConnectionId, user.Code);
                    await Clients.Group(user.Code).ChangeState(room);
                    return room;
                }
            }
            return null;
        }

        public async Task<Character> ChangeCharacter(CharacterName characterName)
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user != null)
            {
                Room room = Server.GetRoom(user.Code);
                if (room != null && !room.IsStarted)
                {
                    Player player = room.GetPlayer(user.Id);
                    if (player != null && !player.IsReady && player.ChangeCharacter(characterName))
                    {
                        await Clients.Group(room.Code).ChangeState(room);
                        return player.Character;
                    }
                }
            }
            return null;
        }

        public async Task ChangeReady()
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user == null)
                return;
            Room room = Server.GetRoom(user.Code);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(user.Id);
                if (player != null)
                {
                    player.ChangeReady();
                    await Clients.Group(room.Code).ChangeState(room);
                }
            }
        }

        public async Task StartGame()
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user == null)
                return;
            Room room = Server.GetRoom(user.Code);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(user.Id);
                if (player != null && room.Start())
                    await Clients.Group(room.Code).ChangeState(room);
            }
        }

        public GameMap GetGameMap()
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user != null)
            {
                Room room = Server.GetRoom(user.Code);
                if (room != null)
                    return room.GetGameMap();
            }
            return null;
        }

        private bool CheckWeaponAvailability(Item weapon, WeaponType weaponType)
        {
            return weapon.Name == ItemName.Empty || (weapon.Type == ItemType.Weapon &&
                (weapon.WeaponType == weaponType || weaponType == WeaponType.None));
        }

        public async Task<bool> ChangeInventory(Inventory inventory)
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user == null)
                return false;
            Room room = Server.GetRoom(user.Code);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(user.Id);
                if (player != null && player.IsTurn)
                {
                    List<ItemName> oldItems = new List<Item>(player.Inventory.Items)
                    {
                        player.Inventory.LeftWeapon,
                        player.Inventory.RightWeapon,
                        player.Inventory.Armor
                    }.Select(i => i.Name).Where(i => i != ItemName.Empty).ToList();

                    List<ItemName> newItems = new List<Item>(inventory.Items)
                    {
                        inventory.LeftWeapon,
                        inventory.RightWeapon,
                        inventory.Armor
                    }.Select(i => i.Name).Where(i => i != ItemName.Empty).ToList();

                    if (oldItems.Count != newItems.Count)
                        return false;

                    oldItems.Sort();
                    newItems.Sort();

                    for (int i = 0; i < oldItems.Count; i++)
                    {
                        if (oldItems[i] != newItems[i])
                            return false;
                    }

                    WeaponType weaponType = player.Character.WeaponType;
                    Inventory newInventory = new Inventory
                    {
                        LeftWeapon = Item.CreateItem(inventory.LeftWeapon.Name),
                        RightWeapon = Item.CreateItem(inventory.RightWeapon.Name),
                        Armor = inventory.Armor,
                        Items = inventory.Items.Where(i => i.Name != ItemName.Empty)
                            .Select(i => i.Type == ItemType.Armor ? i : Item.CreateItem(i.Name)).ToList(),
                        Drop = player.Inventory.Drop
                    };
                    if (!CheckWeaponAvailability(newInventory.LeftWeapon, weaponType) ||
                        !CheckWeaponAvailability(newInventory.RightWeapon, weaponType) ||
                        !(newInventory.Armor.Type == ItemType.Armor || newInventory.Armor.Name == ItemName.Empty))
                        return false;
                    int hands = (int)newInventory.LeftWeapon.WeaponHanded + (int)newInventory.RightWeapon.WeaponHanded;
                    if (hands > player.Hands)
                        return false;

                    player.Inventory = newInventory;
                    await Clients.Group(room.Code).ChangeState(room);
                    return true;
                }
            }
            return false;
        }

        public int RollTheDice()
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user != null)
            {
                Room room = Server.GetRoom(user.Code);
                if (room != null && room.IsStarted)
                {
                    Player player = room.GetPlayer(user.Id);
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
            }
            return -1;
        }

        public async Task<bool> DoAction(GameAction gameAction)
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user == null)
                return false;
            Room room = Server.GetRoom(user.Code);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(user.Id);
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
                            GameActionType.DropItem => () => DropItem(gameAction),
                            GameActionType.UseAbility => () => UseAbility(room, gameAction),
                            GameActionType.TakeTheFlag => () => TakeTheFlag(room, gameAction),
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
                            if (player.WithFlag && player.Position.Equals(player.SpawnPoint))
                            {
                                player.Score++;
                                room.DropTheFlag(player);
                                room.FlagPosition = room.GetGameMap().FlagSpawnPoint;
                                if (room.CheckWinner(player))
                                    Server.RemoveRoom(room.Code);
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
                Player player = gameAction.CurrentPlayer;
                Vector targetPosition = gameAction.TargetPosition;
                int x = targetPosition.X;
                int y = targetPosition.Y;
                if ((player.Position.CheckAvailability(targetPosition, 1) ||
                    player.Position.CheckAvailability(targetPosition, dice + player.Speed)) &&
                    targetPosition.IsInMap(room.GetGameMap()) &&
                    room.GetGameMap().Map[x, y] != Cell.None && room.GetGameMap().Map[x, y] != Cell.Wall)
                {
                    player.Position = targetPosition;
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

                    int resultDamage = playerAttackDamage - enemyArmorStrength - enemyArmorBuff;
                    if (resultDamage > 0)
                        targetPlayer.Health -= resultDamage;

                    if (enemyArmor.Type == ItemType.Armor)
                    {
                        enemyArmor.ArmorDurability--;
                        if (enemyArmor.ArmorDurability <= 0)
                            targetPlayer.Inventory.Armor = new EmptyItem();
                    }

                    if (targetPlayer.WithFlag)
                        room.DropTheFlag(targetPlayer);

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
                            item.Get(gameAction.CurrentPlayer);

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

        private bool DropItem(GameAction gameAction)
        {
            return gameAction.CurrentPlayer.Inventory.DropItem(gameAction.Item, gameAction.CurrentPlayer);
        }

        private bool UseAbility(Room room, GameAction gameAction)
        {
            Ability ability = gameAction.CurrentPlayer.Character.Abilities.Find(a => a.Name == gameAction.Ability.Name);
            if (ability != null && ability.Type == AbilityType.Active && ability.IsActive)
                return ability.Use(room, gameAction);
            return false;
        }

        private bool TakeTheFlag(Room room, GameAction gameAction)
        {
            return room.TryTakeTheFlag(gameAction.CurrentPlayer);
        }

        public async Task<bool> EndTurn()
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user == null)
                return false;
            Room room = Server.GetRoom(user.Code);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(user.Id);
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
            User user = Server.GetUser(Context.ConnectionId);
            if (user != null)
            {
                Room room = Server.GetRoom(user.Code);
                if (room != null)
                {
                    Server.RemoveUser(Context.ConnectionId);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Code);
                    string id = user.Token == null ? Context.ConnectionId : user.Id;
                    if (room.Disconnect(id))
                        Server.RemoveEmptyRoom(room);
                    await Clients.Group(room.Code).ChangeState(room);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public int GetAttackRadius()
        {
            User user = Server.GetUser(Context.ConnectionId);
            if (user != null)
            {
                Room room = Server.GetRoom(user.Code);
                if (room != null && room.IsStarted)
                {
                    Player player = room.GetPlayer(user.Id);
                    if (player != null && player.IsTurn)
                        return CalculateAttackRadius(player);
                }
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

            return CalculateAttackDamage(handsAttackDamage, 0, player.MultiplyDamage);
        }

        private int CalculateAttackDamage(int weaponDamage, int damageBuff, double damageMultipluyer)
        {
            return (int) Math.Round((weaponDamage + damageBuff) * damageMultipluyer);
        }
    }
}