using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class rubyPicker : MonoBehaviour
{

    private float ruby = 0;

    public TextMeshProUGUI textRuby;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "ruby")
        {
            ruby++;
            textRuby.text = ruby.ToString();
            Destroy(other.gameObject);
        }
    }
}
