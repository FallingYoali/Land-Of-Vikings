﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    //Esta es la forma correcta de mostrar variables privadas en el inspector
    //No se deben hacer public variables que no queremos sean accesibles desde otras clases
    [SerializeField]
    private string sceneToLoad;
    [SerializeField]
    private Text percentText;
    [SerializeField]
    private Image progressImage;
    // Start is called before the first frame update
    void Start()
    {
        //Iniciamos una corrutina
        StartCoroutine(LoadScene());
    }

    //Corrutina
    IEnumerator LoadScene()
    {

       // yield return new WaitForSeconds(1.5f); // Solamente para que se visualice la escena de carga, pero esto en el script final se le quitara
        AsyncOperation loading;
        //Inciamos la carga asincrona de la escena y guardamos el proceso en 'loading'
        loading = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);

        //Bloqueamos el salto automatico entre escenas para asegurarnos el control durante el proceso
        loading.allowSceneActivation = false;

        //Cuando la escena llega al 90% de carga, se produce el cambio de escena
        while (loading.progress < 0.9f)
        {
            //Actualizamos el % de carga de una forma optima
            //(concatenar con + tiene un alto coste en el rendimiento)
            percentText.text = string.Format("{0}%", loading.progress * 100);

            //Actualizamos la barra de carga
            progressImage.fillAmount = loading.progress;

            //Esperamos un frame
            yield return null;
        }

        //Mostramos la carga como finalizada
        percentText.text = "100%";
        progressImage.fillAmount = 1;

        //Activamos el salto de escena
        loading.allowSceneActivation = true;
    }
}
