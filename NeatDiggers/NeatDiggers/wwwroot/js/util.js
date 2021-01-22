import { GLTFLoader } from '../lib/three/examples/jsm/loaders/GLTFLoader.js';
const loader = new GLTFLoader();

export function checkAvailability(startPoint, targetPoint, radius) {
    let x = targetPoint.x - startPoint.x;
    let y = targetPoint.y - startPoint.y;
    return radius >= Math.round(Math.sqrt(x * x + y * y));
}

export async function modelLoader(name) {
    return new Promise((resolve, reject) => {
        loader.load("../../StaticFiles/models/" + name + ".glb", data => resolve(data.scene.children[0]), null, reject);
    });
}

export const Message = {
    Inventory: {
        Title: "Инвентарь: ",
        LeftWeapon: "Оружие левой руки: ",
        RightWeapon: "Оружие правой руки: ",
        Two: "Двуручное оружие: ",
        Armor: "Броня: ",
        Drop: "Сброс: ",
    },
    Log: "Лог: ",
    TakeOff: "Снять",
    Effects: "Эффекты: ",
    NeedRollDice: "Вам нужно бросить кубик.",
    Health: "Здоровье: ",
    Level: "Уровень: ",
    Score: "Счёт: ",
    WeaponType: "Тип оружия: ",
    Abilities: "Способности: ",
    YouMove: "Ваш ход!",
    ActionRemains: "Осталось очков действий: ",
    ItemLimit: "В инвентаре может быть максимально 6 предметов",
    Button: {
        TakeFlag: "Взять флаг",
        Drop: "Сброс",
        Passive: "Пассивный предмет",
        Use: {
            None: "Использовать",
            Player: "Использовать на игрока",
            Position: "Использовать на позицию",
        },
        Equip: {
            Armor: "Экипировать",
            Left: "Взять в левую руку",
            Right: "Взять в правую руку",
            Two: "Взять в две руки",
        },
        RollDice: "Бросить кубик",
        Dig: "Копать",
        EndTurn: "Закончить ход"
    }
}

export const Conection = {
    Error: {
        Full: {
            Code: "full",
            Message: "Лобби заполнено."
        },
        Started: {
            Code: "started",
            Message: "Эта игра уже начата."
        },
        WrongCode: {
            Code: "wrongCode",
            Description: "По данному коду не существует комнаты."
        },
    }
}

export const Ability = {
    Name: {
        Speed: 0,
        Shuriken: 1,
        Heal: 2,
        Damage: 3,
        Distance: 4,
        Armor: 5,
        Invul: 6,
        ArmorBreak: 7
    },
    Type: {
        Passive: 0,
        Active: 1
    }
}

export const Target = {
    None: 0,
    Player: 1,
    Position: 2
}

export const ItemType = {
    //Event,
    Passive: 0,
    Active: 1,
    Weapon: 2,
    Armor: 3
}

export const WeaponHanded = {
    None: 0,
    One: 1,
    Two: 2
}

export const GameActionType = {
    Move: 0,
    Dig: 1,
    Attack: 2,
    UseItem: 3,
    DropItem: 4,
    UseAbility: 5,
    TakeTheFlag: 6
}

export const WeaponType = {
    'None': 0,
    'Melee': 1,
    'Ranged': 2
}

export function getKeyByValue(object, value) {
    return Object.keys(object).find(key => object[key] === value);
}