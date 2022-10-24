using UnityEngine;

namespace Client
{
    public class AzureMetrica
    {
        public static void LevelStart(string name, int number, int count, int loop)
        {
#if !UNITY_EDITOR
        var data = new LevelStartData
        {
            level_name = name,
            level_number = number,
            level_count = count,
            level_loop = loop
        };

        var json = JsonUtility.ToJson(data).ToLower();

        AppMetrica.Instance.ReportEvent("level_start", json);
        AppMetrica.Instance.SendEventsBuffer();
#else
            Debug.Log("---- level_start send");
#endif

        }

        public static void LevelFinish(string name, int number, int count, int loop,
            string result, int time, int progress = 100)
        {
#if !UNITY_EDITOR
        var data = new LevelFinishData
        {
            level_name = name,
            level_number = number,
            level_count = count,
            level_loop = loop,
            result = result,
            time = time,
            progress = progress,
            
        };

        var json = JsonUtility.ToJson(data).ToLower();


            AppMetrica.Instance.ReportEvent("level_finish", json);
            AppMetrica.Instance.SendEventsBuffer();
#else
            Debug.Log("---- level_finish send");
#endif
        }


        public static void TutorialShowed(string level_name, int level_number, string step_name)
        {
#if !UNITY_EDITOR
        var data = new TutorialPartData
        {
            level_name = level_name,
            level_number = level_number,
            step_name = step_name,
        };
        var json = JsonUtility.ToJson(data).ToLower();
        AppMetrica.Instance.ReportEvent("tutorial", json);
#else
            Debug.Log("---- TutorialShowed send");
#endif
        }

        public static void EnemyMeet(string level_name, int level_number, string enemy_name, string point, bool is_boss, bool i_lose)
        {
#if !UNITY_EDITOR
        var data = new EnemyMeetData
        {
            level_name = level_name,
            level_number = level_number,
            enemy_name = enemy_name,
            point = point,
            is_boss = is_boss,
        };
        var json = JsonUtility.ToJson(data).ToLower();
        AppMetrica.Instance.ReportEvent("enemy_meet", json);
#else
            Debug.Log("---- enemy_meet send");
#endif
        }

        class TutorialPartData
        {
            public string level_name;
            public int level_number;
            public string step_name;
        }
        class EnemyMeetData
        {
            public string level_name;
            public int level_number;
            public string enemy_name;
            public string point;
            public bool is_boss;
            public bool i_lose;
        }


        class LevelStartData
        {
            public string level_name;
            public int level_number;
            public int level_count;
            public int level_loop;
            public string level_diff = "ease";
            public bool level_random = false;
            public string level_type = "normal";
            public string game_mode = "classic";
        }
        class LevelFinishData
        {
            public string level_name;
            public int level_number;
            public int level_count;
            public int level_loop;
            public string level_diff = "ease";
            public bool level_random = false;
            public string level_type = "normal";
            public string game_mode = "classic";
            public string result;
            public int time;
            public int progress = 100;
            public int Continue = 0;
        }
    }
}