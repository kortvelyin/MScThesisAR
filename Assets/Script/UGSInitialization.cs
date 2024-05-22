using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class UGSInitialization : MonoBehaviour
{
  
async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
           // await VivoxService.Instance.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }



}

