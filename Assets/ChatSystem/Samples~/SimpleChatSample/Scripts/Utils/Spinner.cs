using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat
{
    [AddComponentMenu("SocketChat/SimpleChatSample/Utils/Spinner")]
    public class Spinner : MonoBehaviour
    {
        private Image _image;
        private YieldInstruction _wait = new WaitForEndOfFrame();
        
        private float _fillAmount = 0f;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }


        private void OnEnable()
        {
            StartCoroutine(Spin());
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator Spin()
        {
            while (Application.isPlaying)
            {
                _fillAmount += Time.deltaTime * (_image.fillClockwise ? 1f : -1f);
                _image.fillAmount = _fillAmount;
                
                if (_fillAmount >= 1f || _fillAmount <= 0f)
                {
                    _image.fillClockwise = !_image.fillClockwise;
                }
                
                yield return _wait;
            }
        }
    }
}
