using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LocalizationManager: MonoBehaviour
{
    public static LocalizationManager instance ;

    private Dictionary<string, string> EngRu;
    private Dictionary<string, string> RuEng= new Dictionary<string, string>();

    public string language = "Eng";

    public delegate void ChangeLanguageDel();
    public event ChangeLanguageDel ChangeEvent;

    public Text label;
    public GameObject settingsPanel;
    Dropdown dropDown;

    public Font RuFont;
    public Font EngFont;

    public void SetReferences()
    {
        if (GameObject.FindGameObjectWithTag("SettingsPanel") == null) return;

        settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");
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
            {"The host has left the room","Хост покинул игру" }



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

        if (language == "Eng")
            localizedText = RuEng;
        else
            localizedText = EngRu;

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
