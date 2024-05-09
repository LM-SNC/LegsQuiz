namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;


        public int MaxScore = 0;
        public int AllTimeScore = 0;

        // public SavesYG()
        // {
        //     openLevels[1] = true;
        // }
    }
}