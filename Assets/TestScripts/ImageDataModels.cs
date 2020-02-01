using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ImageDataModels
{
    public string action {get;set;}
    [JsonProperty("image-data")]
    public List<ImageData> data {get;set;}
}

public class ImageData
{
    public float x {get;set;}
    public float y {get;set;}
    public bool dragging {get;set;}
    public string color {get;set;}
    public float size {get;set;}
}