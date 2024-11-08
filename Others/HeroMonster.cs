/*
    角色扮演游戏战斗挑战
    必须使用 do-while 语句或 while 语句作为外部游戏循环。
    英雄和怪物在开始时的生命值都为 10。
    所有攻击的值都介于 1 到 10 之间。
    英雄先发起攻击。
    打印怪物损失的生命值，以及剩余的生命值。
    如果怪物的生命值大于 0，则它会攻击英雄。
    打印打印英雄损失的生命值，以及剩余的生命值。
    继续此攻击顺序，直到怪物或英雄任意一方的生命值为零或更低。
    打印胜利者。
*
    Monster was damaged and lost 1 health and now has 9 health.
    Hero was damaged and lost 1 health and now has 9 health.
    Monster was damaged and lost 7 health and now has 2 health.
    Hero was damaged and lost 6 health and now has 3 health.
    Monster was damaged and lost 9 health and now has -7 health.
    Hero wins!
*/

internal class HeroMonster {
    internal static void RunHeroMonster() {
        int hero = 10;
        int monster = 10;
        int attack;
        Random random = new Random();

        do {
            attack = random.Next(1, 11);
            monster -= attack;
            Console.WriteLine($"Monster was damaged and lost {attack} health and now has {monster} health.");

            if (monster >= 0) {
                attack = random.Next(1, 11);
                hero -= attack;
                Console.WriteLine($"Hero was damaged and lost {attack} health and now has {hero} health.");
            }

        } while (hero > 0 && monster > 0);

        if (hero >= monster) {
            Console.WriteLine("Hero wins!");
        } else {
            Console.WriteLine("Monster wins!");
        }
    }
}