using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkcupGames;
using DG.Tweening;

public interface ObserverListener 
{
    public abstract void NotifyEvent(ObserverEvent e);
}