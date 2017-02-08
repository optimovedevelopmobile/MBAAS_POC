using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


public class QueryPerfCounter
{

[DllImport("KERNEL32")]
private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

[DllImport("Kernel32.dll")]
private static extern bool QueryPerformanceFrequency(out long lpFrequency);

    private long _frequency = 0;
    private long _start = 0;
    private long _stop = 0;
    private int _iterations = 0;
    Decimal _multiplier = new Decimal(1.0e9);
    

        public QueryPerfCounter()
        {
            if (QueryPerformanceFrequency(out _frequency) == false)
            {
                // Frequency not supported
                throw new Win32Exception();
            }
        }

        public void Start()
        {
            _start = 0; 
            QueryPerformanceCounter(out _start);
        }

        public void Stop()
        {
            _stop = 0;
            QueryPerformanceCounter(out _stop);
        }

        public double Duration(int iterations)
        {
        return ((((double)(_stop - _start)* (double) _multiplier) / (double) _frequency)/iterations);
        }
}