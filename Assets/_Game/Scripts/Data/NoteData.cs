using System.Collections.Generic;
using Newtonsoft.Json;
using SMT3.Notes;

namespace SMT3.Data
{
    [System.Serializable]
    public class RootNoteData
    {
        [JsonProperty("notes")]
        public List<NoteData> Notes = new();
        [JsonProperty("songMeta")]
        public SongMeta SongMeta;
        [JsonProperty("format")]
        public string Format;
    }
    
    [System.Serializable]
    public class NoteData
    {
        [JsonProperty("lane")]
        public int Lane;
        [JsonProperty("time")]
        public double Time;
        [JsonProperty("type")]
        public NoteType Type;
        [JsonProperty("metas")]
        public List<MetaData> Metas = new();
        [JsonProperty("controls")]
        public List<ControlData> Controls = new();
        [JsonProperty("duration")]
        public float Duration;
    }

    [System.Serializable]
    public class MetaData
    {
        [JsonProperty("key")]
        public string Key;
        [JsonProperty("value")]
        public string Value;
    }

    [System.Serializable]
    public class ControlData
    {
        [JsonProperty("lane")]
        public int Lane;
        [JsonProperty("time")]
        public float Time;
    }
    
    [System.Serializable]
    public class SongMeta
    {
        [JsonProperty("bpm")]
        public int Bpm;
        [JsonProperty("nLanes")]
        public int NLanes;
        [JsonProperty("visualSpeed")]
        public float VisualSpeed;
        [JsonProperty("audioDuration")]
        public float AudioDuration;
    }
}