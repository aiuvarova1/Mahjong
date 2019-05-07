using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;

    private Dictionary<string, string> EngRu;
    private Dictionary<string, string> RuEng = new Dictionary<string, string>();

    public string language = "Eng";

    public delegate void ChangeLanguageDel();
    public event ChangeLanguageDel ChangeEvent;

    public Text label;
    public GameObject settingsPanel;
    public Dropdown dropDown;

    public Font RuFont;
    public Font EngFont;

    public void SetReferences()
    {
        if (GameObject.FindGameObjectWithTag("SettingsPanel") == null) return;

        settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");
        

        PlayerPrefs.instance.nameText= GameObject.FindObjectsOfType<InputField>()[1];

        PlayerPrefs.instance.SetReferences();

        dropDown = settingsPanel.GetComponentInChildren<Dropdown>();

        label = dropDown.GetComponentInChildren<Text>();


        if (language == "Eng")
            dropDown.value = 0;
        else
            dropDown.value = 1;

        dropDown.onValueChanged.AddListener(delegate { OnLanguageChanged(); });

        settingsPanel.SetActive(false);
    }

    public void ChangeFont(ref Text text)
    {
        if (language == "Eng")
            text.font = EngFont;
        else
            text.font = RuFont;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        EngRu = new Dictionary<string, string>()
        {
            {"Play","Играть" },
            {"Settings","Настройки" },
            {"Rules","Правила" },
            {"Exit","Выход" },
            {"Language","Язык" },
            {"Rooms","Комнаты" },
            {"Create\nroom","Создать\nкомнату" },
            {"Back","Назад" },
            {"Room name:","Название комнаты" },
            {"Error","Ошибка" },
            {"Chow","Чоу" },
            {"Pung","Панг" },
            {"Kong","Конг" },
            {"Mahjong","Маджонг" },
            {"Pass","Пас" },
            {"Your wind:","Ваш ветер:" },
            {"East","Восток" },
            {"South","Юг" },
            {"West","Запад" },
            {"North","Север" },
            {"Quit","Выйти" },
            {"Players","Игроки" },
            {"Music","Музыка" },
            {"Sounds","Звуки" },
            {"Name","Имя" },
            {"Start","Начать" },
            {"You are not the next player","Вы не следующий игрок" },
            {"Chow can not be declared of winds or dragons","Чоу не может быть из ветров или драконов" },
            {"No tiles for chow","Нет костей для Чоу" },
            {"It's not your turn","Не Ваша очередь" },
            {"No tiles for Pung","Нет костей для Панга" },
            {"No tiles for Kong","Нет костей для Конга" },
            {"No MahJong","Нет костей для Маджонга" },
            {"On","Вкл" },
            {"Off","Выкл" },
            {"View","Вид" },
            {"Total","Итог" },
            {"Points","Очки" },
            {"Old score","Счёт" },
            {"New score","Новый счёт" },
            {"OK","ОК" },
            {"Combinations","Комбинации" },
            {"Continue","Продолжить" },
            {"The host has left the room","Хост покинул игру" },
            {"has left the room","покинул игру" },
            {"Waiting for the end of the game...","Ожидаем конца игры..." },
            {"Your move","Ваш ход" },
            {"Your turn","Ваша очередь" },
            {"Draw!","Ничья!" },
            {"Please, enter your name","Пожалуйста, введите Ваше имя" },
            {"Enter","Ввод" },
            {"You: ","Вы: " },
            {"Your wind","Ваш ветер" },
            {"Tiles\n","Кости\n" },
            {"A set of MahJong tiles consists of 144 tiles.There are 3 suits: Bamboos,Dots and Symbols.\n",
                "Игра ведётся набором из 144 костей. В игре 3 масти: Бамбуки, Доты, Символы. \n" },
            {"Bamboos\n","Бамбуки\n" },
            {"Dots\n","Доты\n" },
            {"Symbols\n","Символы\n" },
            {"Each of suit tiles from 1 to 9 has 3 more copies.\nBesides that there are 4 winds: East, South, West, North,\n",
            "Костей каждой масти от 1 до 9 по 4 штуки.\n Кроме того есть 4 ветра: Восток, Юг, Запад, Север, \n"},
            {"Winds\n","Ветра\n" },
            {"Dragons\n","Драконы\n" },
            {"and 3 dragons - Red, Green and White, 4 copies of each type.\n","и 3 дракона - Красный, Зеленый и Белый, по 4 штуки каждого вида.\n" },
            {"There are also 8 additional tiles: 4 seasons and 4 flowers.\n","Также есть 8 дополнительных костей: 4 сезона и 4 цветка.\n" },
            {"Seasons\n","Сезоны\n" },
            {"Flowers\n","Цветы\n" },
            {@"There are 4 players in the game. The players seat themselves in the clockwise order N - W - S - E.
Notice that these are NOT the standard compass positions. Each player owns a corresponding wind. The winds are assigned randomly at the beginning of the game.

East the prevailing wind and the key position since this player starts, scores double and pays double for the round.  For the each subsequent round, the positions change in one of two ways:

-If East wind went out in the previous round, then the positions stay the same and the player who was East wind remains the same for another round

-If one of the other winds went out in the previous round, the wind positions rotate in an anti-clockwise fashion so that the player who was South wind in the previous round becomes East wind.
",
            "В игре 4 игрока. Они сидят по часовой стрелке в порядке N-W-S-E (Север-Запад-Юг-Восток). Следует заметить, что это НЕ стандартные компасные позиции. Каждый игрок \"владеет\" соответствующим ветром. Ветра присваиваются случайным образом в начале игры.\n\n " +
            "Восток - преимущественный ветер и с него начинается игра. Он выигрывает вдвое больше очков, но и платит победителю раунда вдвое больше. После каждого раунда позиции ветров меняются одним из двух способов: \n\n" +
            "-Если Восток победил в раунде, ветра остаются прежними и игрок, который был Востоком, им остается на следующий раунд.\n\n" +
            "-Если один из других ветров победил, ветра смещаются против часовой стрелки: тот, кто был Югом, становится Востоком, и т.д.\n"},
            {"Wall\n","Стена\n" },
            {"In the beginning all the tiles are shuffled thoroughly face down. Once done, each player takes 36 tiles and positions them in a wall, 2 tiles high and 18 tiles long.  The tiles should have the long sides and be face down.  Each wall should lie in front of each player running from left to right.  The four walls are then pushed together to form a square symbolising the Great Wall of China.\n",
            "Вначале все фишки переворачиваются лицевой стороной вниз и перемешиваются. После этого каждый игрок берет 36 костей и выкладывает их стеной высотой 2 и длиной 18. Все кости должны быть перевернуты. Каждая часть стены располагается перед каждым игроком, слева направо. Затем все 4 стены сдвигаются в центр, формируя кадрат, символизирующий Великую Китайскую Стену.\n"},
            {"Full Wall\n","Полная Стена\n" },
            {"The deal\n","Раздача\n" },
            {@"The place where the Wall will be broken is chosen randomly (traditionally with the help of dices). Firstly, one of the 4 parts of the wall is chosen. After that  a random pair of tiles is removed and placed on the right on top of the previous two tiles. These two tiles are called free tiles and indicate the end of the Wall.

Starting after the break (i.e. continuing in a clockwise direction around the wall), four tiles are dealt to each player in turn starting with East and working anti-clockwise until each player has 12 tiles.

The fourth time each player (in the same order) takes only one tile. That completes the deal, each player has 13 tiles.

After that each player, starting from the East, opens flowers and seasons(if he has ones) one by one and takes free tile instead of each of them. That process continues until the player has no flowers and seasons. Than the turn goes to the next player (E-S-W-N) until each player has no flowers or seasons. If free tiles end, the pair at the end of the wall is put in the same way.

After the deal the leader(East) takes the 14-th tile from the beginning of the wall.
", "Место, где Стена будет разобрана, выбирается случайно ( традиционно с помощью игральных кубиков). Сначала выбирается одна из 4-х частей стены. Затем случайная пара костей вынимается и располагается справа, третьим рядом, поверх соседних костей. Эти две кости называются свободными и задают конец Стены.\n\n" +
"Начиная слева от места разбора Стены, кости раздаются по 4 штуки каждому игроку по очереди, начиная с Востока, пока у каждого из игроков не будет по 12 костей.\n\n" +
"В четвертый раз каждый игрок (в том же порядке) берет по одной кости. На этом завершается раздача, у каждого игрока по 13 костей.\n\n" +
"После этого каждый игрок (в том же порядке) выкладывает перед собой цветы и сезоны ( если они у него есть ) по одному и берет вместо каждой кости одну из свободных. Этот процесс продолжается, пока у игрока не останется цветов/сезонов. Затем очередь переходит дальше, пока у каждого игрока не останется цветов и сезонов. Если в какой-то момент свободные кости кончаются, на их место с конца Стены кладутся новые.\n\n" +
"После раздачи лидер (Восток) берёт 14-ю кость с начала Стены." },
            {"After the deal\n","После раздачи\n" },
            {"The game\n","Игра\n" },
            {@"<b>Objective</b>

A player generally tries to collect sets of tiles.  The 3 basic sets are as follows:

A <b>Pung</b> - a set of 3 identical tiles  e.g. 3 x Red Dragons, 3 x dots
A <b>Kong</b> - a set of 4 identical tiles.  e.g. 4 x Eight of Bamboos or 4 x North Winds.
A <b>Chow</b> - a run of 3 tiles in the same suit.  A Chow does not score and so is only helpful because it can contribute to a hand that allows a player to call Mah Jong.

The primary aim of the game is to collect such tiles that allow a player to call <b>MahJon</b> and go out.  In order to do this, a player must achieve a combination consisting of a pair, and 4 Pungs, Kongs or Chows.

<b>The Play</b>

The first turn is made by East who discards one tile by placing it face-up on the table inside the remainder of the walls.  Each subsequent turn is made by a player taking a tile, optionally playing a tile combination and then discarding a tile.  However, which player takes the turn and from where the tile is taken, varies.

After each discard, any player who has 2 or 3 tiles that match the discarded tile may take the next turn by calling MahJong, Pung or Kong.  Such a player, takes the discard and plays the resulting Pung or Kong on the table in front of him or, in the case of MahJong, takes the discard and declares all tiles in hand, thus finishing the game. If the player declares Kong, he should take a free tile before the discard.

If no player calls Mah Jong, Pung or Kong, then the player to the right of the player who just discarded takes the next turn.  This player may, if he has 2 tiles that can be matched with the discarded tile to form a Chow, call a Chow (the player must then take the discard and play the resulting Chow). 

-The declared combinations are called opened and the ones the player has in his hand are called closed.

-A player who wants to declare Pung or Kong must cede the discard to the player who is going to declare MahJong. 

-If player has 4 closed matching tiles he can declare closed Kong (if it is his turn to discard a tile). He must open all the four tiles.  After that he takes a free tile and makes a discard. It is not obligatory 'cause the fourth tile can be used in other combination. However, if the closed kong is not declared, in the end it will be accounted as closed Pung.

-If player has an opened Pung and has a closed matching tile, he can declare an opened Kong(if it is his turn to discard a tile). In this case he must open this tile and form a Kong. After that a player takes a free tiles and makes a discard.

-Tiles that have been discarded, unless they are picked up in the following turn, are dead tiles and take no further part in the game.

-If a player receives a flower or season from the Wall, he openes it and takes a free tile from the Wall. There must be no flowers or seasons on hand before the discard.
", "<b>Цель</b>\n\n" +
"Каждый игрок во время игры собирает комбинации. Комбинации бывают 3-х типов:\n\n" +
"<b>Панг</b> - набор из 3-х одинаковых костей, например, 3 Красных дракона\n" +
"<b>Конг</b> - набор из 4-х одинаковых костей, например 4 восьмёрки Бамбуков\n" +
"<b>Чоу</b> - последовательность из 3-х костей одной масти. Чоу ничего не стоят, но с их помощью можно собрать Маджонг.\n\n" +
"Главная цель игры - собрать набор костей, который позволит объявить Маджонг. Для этого игрок должен собрать набор, состоящий из пары одинаковых костей и 4-х Пангов, Конгов или Чоу.\n\n" +
"<b>Игра</b>\n\nПервый ход делает Восток, который сносит лишнюю 14-ю кость лицевой стороной вверх на стол внутри оставшейся Стены. Каждый следующий игрок вновь берет 14-ю кость со стены и делает снос.\n\n" +
"После каждого сноса люьой игрок, имеющий 2 или 3 таких же кости может объявить Панг, Конг или Маджонг (на Маджонг кость можно взять в Чоу или в пару). Такой игрок забирает снесенную кость и выкладывает комбинацию на стол перед собой. В случае Маджонга объявляется конец игры.\n\n" +
"Если никто из игроков не объявил Панг, Конг или Маджонг, игрок, сидящий справа от сделавшего снос, может объявить Чоу ( тогда он также забирает кость и выкладывает комбинацию, а затем делает снос). Иначе он должен взять кость со стены и сделать ход.\n\n" +
"-Объявленные комбинации, объявленные с помощью чужой снесенной кости, называются открытыми, а те, что на руке, - закрытыми.\n\n" +
"-Игрок, который хочет объявить Панг, Конг или Чоу, обязан уступить кость игроку, который хочет объявить Маджонг.\n\n" +
"-Если у игрока на руках есть 4 одинаковых кости, то он может объявить закрытый Конг (если его очередь сносить кость). Он должен выложить комбинацию на стол, взять свободную кость и сделать снос. Игрок не обязан объявлять закрытый Конг, т.к. 4-я кость может участвовать в другой комбинации. Тем не менее, пв таком случае при подсчете очков комбинация будет считать как закрытый Панг.\n\n" +
"-Если у игрока собран открытый Панг и у него на руках есть 4-я такая же кость, он может объявить открытый Конг (если его очередь делать снос). Он должен доложить кость к Пангу, взять свободную кость и сделать снос.\n\n" +
"-Кости, которые были снесены, но не были взяты на комбинации, считаются мертвыми и не участвуют в дальнейшей игре.\n\n" +
"-Если игрок получает цветок или сезон со Стены, он открывает его и берет взамен свободную кость. Перед сносом у игрока не должно быть цветов и сезонов.\n"},
            {"Game process\n","Игровой процесс\n" },
            {"End of the game\n","Конец игры\n" },
            {@"If there are 7 pairs of tiles in the Wall without free ones then the game is declared a draw and no scores are made.  The tiles are shuffled again and game is restarted with the same player as East wind.

In the end of the game all the players open their tiles and calculate points.

The player who went Mah Jong is then paid by the other players the amount scored by his hand.  This means that the player who gets Mah Jong always wins the round, even if other players have scored greater amounts.  If East wins, the others pay double. If not, East pays double.  

Each losing player pays any other losing player with a greater value hand, the difference between the two hands, with East paying and/or receiving double the difference.
","Если в стене остается 7 пар костей, не считая свободных, объявляется ничья и очки не подсчитываются. Кости вновь перемешиваются и игра начинается заново с тем же Востоком.\n\n" +
"В конце игры все игроки открывают кости и подсчитывают очки.\n\n" +
"Игрок, объявивший Маджонг, выигрывает у остальных полную стоимость своей комбинации. Это означает, что победитель всегда остается в выигрыше, даже если другие игроки набрали больше очков. Если Восток побеждает, остальные платят удвоенную сумму. Иначе вдвое больше платит Восток.\n\n" +
"Каждый проигравший игрок выплачивает остальным проигравшим разность между своей суммой очков и чужой, Восток платит/получает вдвое больше.\n" },
            {"Points\n","Очки\n" },
            {@"<b>Combinations  (Opened/Closed)</b>

-Pung of 2-8 (2/4)
-Pung of 1,9,winds or dragons(4/8)

-Kong of 2-8 (8/16)
-Pung of 1,9,winds or dragons(16/32)

-Pair of dragons/player's own wind/prevailing wind (2/2)

-For MahJong - 20
-For drawing the winning tile from the wall - 2

<b>Doubles</b>

-Pung or Kong of the player's own Wind (concealed or exposed)
	
-Pung or Kong of the prevailing Wind (concealed or exposed)	

-Pung or Kong of Dragons

-All one suit and some Dragons and/or Winds	

-All major tiles and some Dragons and/or Winds

-All Dragons and/or winds (x8)

-All one suit (x8)

<b>Doubles only for MahJong</b>

-No Chows	

-Non-scoring hand (4 chows and a pair)	
		
-going Mah Jong with the last tile from the free tiles

-the last tile for MahJong was a burglary of an opened Kong

<b>Flowers and seasons(are calculated after all the other points and doubles)</b>

- Not own flower or season - 2 

-Own flower or season - 4 (e.g. 1 season gives 4 points for East, 2 - for South and etc)

-All 4 flowers or all 4 seasons - doubles flower/season points
","<b>Комбинации  (Открытые/Закрытые)</b>\n\n" +
"-Панг из 2-8 (2/4)\n-Панг из 1,9,ветров или драконов(4/8)\n\n-Конг из 2-8 (8/16)\n-Конг из 1,9,ветров или драконов (16/32)\n\n" +
"-Пара собстенных ветров/преимущественых ветров/драконов (2/2)\n\n" +
"-За Маджонг - 20\n-За последнюю кость для победы, взятую со Стены, - 2\n\n" +
"<b> Удвоения</b>\n\n -Панг или Конг собственных ветров (закрытый/открытый)\n\n-Панг или Конг преимущественных ветров (открытый или закрытый)\n\n-Панг или Конг драконов\n\n" +
"-Чистая масть с ветрами и драконами\n\n-Только 1,9,ветра и драконы\n\n-Только ветра и драконы (удвоение трижды)\n\n-Чистая масть (удвоение трижды)\n\n" +
"<b>Удвоения только для выигравшего</b>\n\n-Без Чоу\n\n-Мизер (только Чоу и пара)\n\n-Кость на Маджонг взята из свободных\n\n-Кость на Маджонг была ограблением открытого Конга\n\n" +
"<b>Цветы и сезоны (подсчитываются после всех остальных очков и удвоений)</b>\n\n-Не свой цветок/сезон - 2\n\n-Собственный цветок/сезон - 4  ( например 1 сезон/цветок даёт 4 очка Востоку, 2-Югу и т.д.\n\n" +
"-Все 4 цветка/сезона - удвоение очков за цветы и сезоны\n" }






        };

        //    var res = dict
        //.GroupBy(p => p.Value)
        //.ToDictionary(g => g.Key, g => g.Select(pp => pp.Key).ToList());
        //RuEng = new Dictionary<string, string>();

        //foreach (string key in EngRu.Keys)
        //{
        //    Debug.Log(key);
        //    RuEng.Add(EngRu[key], key);
        //}
        RuEng = EngRu.ToDictionary(x => x.Value, x => x.Key);


        // RuEng =( EngRu.ToLookup(pair => pair.Value, pair => pair.Key));
    }

    public string GetLocalizedValue(string key)
    {
        string result = "";
        Dictionary<string, string> localizedText;

        if (key.Length == 0) return "";
        if (language == "Eng")
        {
            Debug.Log(key.Length);
            if (key.Length>0 && (key[0] >= 'a' && key[0] <= 'z') || (key[0] >= 'A' && key[0] <= 'Z'))
                return key;
            localizedText = RuEng;
        }
        else
        {
            if ((key[0] >= 'а' && key[0] <= 'я') || (key[0] >= 'А' && key[0] <= 'Я'))
                return key;
            localizedText = EngRu;
        }



        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }


        return result;

    }

    public void OnLanguageChanged()
    {
        Debug.Log("onlangch");
        Debug.Log(ChangeEvent == null);
        if (ChangeEvent == null) return;
        language = label.text;
        ChangeEvent?.Invoke();
    }
}
