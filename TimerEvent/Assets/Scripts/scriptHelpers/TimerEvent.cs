using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <license>
/// Copyright (C) 2018-07-17 Tiago Penha Pedroso
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
///
/// You should have received a copy of the GNU General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
/// 
/// Version: 1.0
/// </license>

public class TimerEvent
{

    #region Main Attributes: CurrentTime, CurrentTimeInt, ElapsedTime, ElapsedTimeInt, TargetTime & Watcher
    float currentTime;
    public float CurrentTime
    {
        get
        {
            if (TargetTime == 0)
            {
                currentTime = 0;
                return currentTime;
            }
            else
            {
                if (started)
                {
                    if (paused)
                    {
                        initialTime = GetSystemTime();
                    }

                    currentTime = (GetSystemTime() - initialTime) + currentTimeBackup;

                    if (currentTime > TargetTime)
                    {
                        currentTime = TargetTime;
                        return currentTime;
                    }
                    else
                    {
                        return currentTime;
                    }
                }

                currentTime = 0;
                return currentTime;
            }
        }
    }
    public int CurrentTimeInt { get { return (int)CurrentTime; } }

    float elapsedTime;
    public float ElapsedTime
    {
        get
        {
            if (TargetTime == 0)
            {
                elapsedTime = 0;
                return elapsedTime;
            }
            else
            {
                elapsedTime = targetTime - CurrentTime;

                if (elapsedTime < 0)
                {
                    return 0;
                }
                else
                {
                    return elapsedTime;
                }
            }
        }
    }
    public int ElapsedTimeInt { get { return (int)ElapsedTime; } }

    float targetTime;
    public float TargetTime
    {
        get { return targetTime; }

        set
        {
            if (value >= 0)
            {
                targetTime = value;
            }
        }
    }

    List<ITimerWatcher> timerWatchers;
    public List<ITimerWatcher> TimerWatchers { get { return timerWatchers; } }
    #endregion

    #region Auxiliary Attributes
    float initialTime;
    float currentTimeBackup;

    bool started;
    bool paused;

    float initialTimeForWaiting;
    float targetTimeForWaiting;
    float currentTimeForWaiting;
    float CurrentTimeForWaiting
    {
        get
        {
            if (targetTimeForWaiting == 0)
            {
                currentTimeForWaiting = 0;
                return currentTimeForWaiting;
            }
            else
            {
                if (started && paused)
                {
                    currentTimeForWaiting = GetSystemTime() - initialTimeForWaiting;

                    if (currentTimeForWaiting > targetTimeForWaiting)
                    {
                        currentTimeForWaiting = targetTimeForWaiting;
                        return currentTimeForWaiting;
                    }
                    else
                    {
                        return currentTimeForWaiting;
                    }
                }

                currentTimeForWaiting = 0;
                return currentTimeForWaiting;
            }
        }

        set
        {
            currentTimeForWaiting = value;
        }
    }
    #endregion

    #region Constructors: (float targetTime),  (float targetTime, bool start),  (float targetTime, ITimerWatcher watcher),  (float targetTime, ITimerWatcher watcher, bool start)
    public TimerEvent(float targetTime)
    {
        TargetTime = targetTime;
        timerWatchers = new List<ITimerWatcher>();
    }

    public TimerEvent(bool start, float targetTime) : this(targetTime)
    {
        if (start)
        {
            Start();
        }
    }

    public TimerEvent(float targetTime, ITimerWatcher firstITimerwatcher) : this(targetTime)
    {
        AddTimerWatcher(firstITimerwatcher);
    }

    public TimerEvent(bool start, float targetTime, ITimerWatcher firstITimerwatcher) : this(targetTime, firstITimerwatcher)
    {
        if (start)
        {
            Start();
        }
    }
    #endregion

    #region Main Methods: Start(),  Pause(),  PauseFor(float waitingTime),  Resume(),  ReachTargetTime(), AddTimerWatcher(ITimerWatcher newITimerWatcher), RemoveTimerWatcher(ITimerWatcher iTimerWatcherToRemove), UpdateTimeCounter()
    public void Start()
    {
        initialTime = GetSystemTime();
        currentTimeBackup = 0;

        started = true;
        paused = false;

        initialTimeForWaiting = 0;
        targetTimeForWaiting = 0;
        CurrentTimeForWaiting = 0;
    }

    public void Pause()
    {
        if (started)
        {
            currentTimeBackup = CurrentTime;
            paused = true;
        }
    }

    public void PauseFor(float waitingTime)
    {
        if (started)
        {
            currentTimeBackup = CurrentTime;
            paused = true;
            initialTimeForWaiting = GetSystemTime();

            if (waitingTime >= 0)
            {
                targetTimeForWaiting = waitingTime;
            }
        }
    }

    public void Resume()
    {
        if (started)
        {
            paused = false;

            initialTimeForWaiting = 0;
            targetTimeForWaiting = 0;
            CurrentTimeForWaiting = 0;
        }
    }

    public bool ReachTargetTime()
    {
        float temp1 = CurrentTime;
        float temp2 = ElapsedTime;
        float temp3 = CurrentTimeForWaiting;

        if (started)
        {
            if (paused && targetTimeForWaiting > 0 && CurrentTimeForWaiting >= targetTimeForWaiting)
            {
                Resume();
            }
            else
            {
                if (TargetTime > 0 && CurrentTime >= TargetTime)
                {
                    NotifyTimerWatchers();
                    started = false;
                    return true;
                }
            }
        }

        return false;
    }

    public void AddTimerWatcher(ITimerWatcher newITimerWatcher)
    {
        timerWatchers.Add(newITimerWatcher);
    }

    public void RemoveTimerWatcher(ITimerWatcher iTimerWatcherToRemove)
    {
        timerWatchers.Remove(iTimerWatcherToRemove);
    }

    public void UpdateTimeCounter()
    {
        float temp1 = CurrentTime;
        float temp2 = ElapsedTime;
        float temp3 = CurrentTimeForWaiting;

        if (started)
        {
            if (paused && targetTimeForWaiting > 0 && CurrentTimeForWaiting >= targetTimeForWaiting)
            {
                Resume();
            }
            else
            {
                if (TargetTime > 0 && CurrentTime >= TargetTime)
                {
                    NotifyTimerWatchers();
                    started = false;
                }
            }
        }
    }
    #endregion

    #region Methods: Override of Object and Overload of Operators
    public override string ToString()
    {
        string message =
                "TimerEvent{\n" +
                "   TargetTime:         " + targetTime + "\n" +
                "   CurrentTime (up):   " + CurrentTime + "\n" +
                "   ElapsedTime (down): " + ElapsedTime + "\n" +
                "   Watcher (Observer): " + TimerWatchers + "\n" +
                "}";

        return message;

    }

    public override int GetHashCode()
    {
        var hashCode = 1727196438;
        hashCode = hashCode * -1521134295 + CurrentTime.GetHashCode();
        hashCode = hashCode * -1521134295 + targetTime.GetHashCode();
        hashCode = hashCode * -1521134295 + paused.GetHashCode();
        return hashCode;
    }

    public override bool Equals(object otherTimerEvent)
    {
        var @tempObject = otherTimerEvent as TimerEvent;
        return @tempObject != null &&
               CurrentTime == @tempObject.CurrentTime &&
               ElapsedTime == @tempObject.ElapsedTime &&
               TargetTime == @tempObject.TargetTime &&
               currentTimeBackup == @tempObject.currentTimeBackup &&
               paused == @tempObject.paused;
    }

    public static bool operator ==(TimerEvent timerEvent1, TimerEvent timerEvent2)
    {
        if (Object.Equals(timerEvent1, null) || Object.Equals(timerEvent2, null)) return false;

        return timerEvent1.Equals(timerEvent2);
    }

    public static bool operator !=(TimerEvent timerEvent1, TimerEvent timerEvent2)
    {
        return !timerEvent1.Equals(timerEvent2);
    }

    public static TimerEvent operator +(TimerEvent timerEvent1, TimerEvent timerEvent2)
    {
        return new TimerEvent(timerEvent1.TargetTime + timerEvent2.TargetTime);
    }

    public static TimerEvent operator -(TimerEvent timerEvent1, TimerEvent timerEvent2)
    {
        return new TimerEvent(timerEvent1.TargetTime - timerEvent2.TargetTime);
    }

    public static TimerEvent operator ++(TimerEvent timerEvent)
    {
        timerEvent.TargetTime = timerEvent.TargetTime + 1;
        return timerEvent;
    }

    public static TimerEvent operator --(TimerEvent timerEvent)
    {
        timerEvent.TargetTime = timerEvent.TargetTime - 1;
        return timerEvent;
    }

    public static TimerEvent operator *(TimerEvent timerEvent1, TimerEvent timerEvent2)
    {
        return new TimerEvent(timerEvent1.TargetTime * timerEvent2.TargetTime);
    }

    public static TimerEvent operator /(TimerEvent timerEvent1, TimerEvent timerEvent2)
    {
        return new TimerEvent(timerEvent1.TargetTime / timerEvent2.TargetTime);
    }
    #endregion

    #region Auxiliary Methods: GetSystemTime(), NotifyWatcher(), DebugValues(bool includeSystemTime)
    float GetSystemTime()
    {
        return Time.time;
    }

    void NotifyTimerWatchers()
    {
        if (TimerWatchers.Count > 0)
        {
            for (int count = 0; count < TimerWatchers.Count; count++)
            {
                TimerWatchers[count].OnReachTargetTime();
            }
        }
    }

    public string DebugValues(bool includeSystemTime)
    {
        StringBuilder buff = new StringBuilder();
        for (int count = 0; count < TimerWatchers.Count; count++)
        {
            if (count == 0)
            {
                buff.Append(TimerWatchers[count].ToString());
            }
            else
            {
                buff.Append(", " + TimerWatchers[count].ToString());
            }
        }

        string message =
                (includeSystemTime ? ("SystemTime: " + GetSystemTime() + "\n") : "") +
                "CurrentTime: " + CurrentTime + "\n" +
                "ElapsedTime: " + ElapsedTime + "\n" +
                "TargetTime: " + TargetTime + "\n" +
                "TimerWatchers: " + buff.ToString() + "\n" +
                "initialTime: " + initialTime + "\n" +
                "currentTimeBackup: " + currentTimeBackup + "\n" +
                "paused? " + paused + "\n" +
                "initialTimeForWaiting: " + initialTimeForWaiting + "\n" +
                "targetTimeForWaiting: " + targetTimeForWaiting + "\n" +
                "CurrentTimeForWaiting: " + CurrentTimeForWaiting + "\n" +
                "ReachTargetTime? " + ReachTargetTime() + "\n";

        return message;
    }
    #endregion

}
