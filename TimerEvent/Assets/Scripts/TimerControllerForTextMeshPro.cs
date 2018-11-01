using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TimerControllerForTextMeshPro : MonoBehaviour, ITimerWatcher
{

    #region Attributes Visible on UnityEditor
    public bool debugTimerValues;
    public bool useInterfaceTimerWatcher;
    public bool useIntegerNumbers;
    public InputField iTargetTime;
    public TextMeshProUGUI tTargetTime;
    public TextMeshProUGUI tCurrentTime;
    public TextMeshProUGUI tElapsedTime;
    public InputField iWaitingTime;
    public TextMeshProUGUI tWaitingTime;
    public TextMeshProUGUI tWatcherEvent;
    public TextMeshProUGUI tSucessReachTargetTime;
    #endregion

    #region Auxiliary Attributes of this Component
    TimerEvent timer;

    const string MESSAGE_SUCESS = "Yes! Reached the target time :)";
    const string MESSAGE_FAILURE = "Ohh no! Wait more time :(";
    #endregion

    #region Messages Methods from MonoBehaviour: Start(), Update(), ...
    void Start() //to initialize this component
    {
        if (useInterfaceTimerWatcher)
        {
            timer = new TimerEvent(0, this);
        }
        else
        {
            timer = new TimerEvent(0);
        }
    }

    void Update()  //to repeat in each frame
    {
        tTargetTime.text = timer.TargetTime.ToString();
        tCurrentTime.text = useIntegerNumbers ? timer.CurrentTimeInt.ToString() : timer.CurrentTime.ToString();
        tElapsedTime.text = useIntegerNumbers ? timer.ElapsedTimeInt.ToString() : timer.ElapsedTime.ToString();
        tWaitingTime.text = iWaitingTime.text;

        if (timer.ReachTargetTime())
        {
            print("TimerEvent.ReachTargetTime()\n\n");
            tSucessReachTargetTime.text = MESSAGE_SUCESS;
        }
        else
        {
            tSucessReachTargetTime.text = MESSAGE_FAILURE;
            tWatcherEvent.text = MESSAGE_FAILURE;
        }

        if (debugTimerValues)
        {
            print(timer.DebugValues(false));
        }
    }

    void FixedUpdate() //to repeat precisely in each frame
    {

    }
    #endregion

    #region Auxiliary Methods

    //private

    #endregion

    #region Public Methods

    public void OnClick_Start()
    {
        print("TimerEvent.Start()\n\n");
        float target = iTargetTime.text == "" ? 0 : float.Parse(iTargetTime.text);
        timer.TargetTime = target;
        timer.Start();
    }

    public void OnClick_Pause()
    {
        float targetPause = iWaitingTime.text == "" ? 0 : float.Parse(iWaitingTime.text);

        if (targetPause > 0)
        {
            print("TimerEvent.PauseFor(" + targetPause + ")\n\n");
            timer.PauseFor(targetPause);
        }
        else
        {
            print("TimerEvent.Pause()\n\n");
            timer.Pause();
        }
    }

    public void OnClick_Resume()
    {
        print("TimerEvent.Resume()\n\n");
        timer.Resume();
    }

    public void OnClick_Increment()
    {
        print("TimerEvent.++(TimerEvent timerEvent)\n\n");
        timer++;
        iTargetTime.text = timer.TargetTime.ToString();
    }

    public void OnClick_Decrement()
    {
        print("TimerEvent.--(TimerEvent timerEvent)\n\n");
        timer--;
        iTargetTime.text = timer.TargetTime.ToString();
    }

    public void OnReachTargetTime()
    {
        print("TimerEvent.NotifyTimerWatchers()\n\n");
        tWatcherEvent.text = MESSAGE_SUCESS;
    }

    #endregion


}//class