STOP
! LCM Variables
GLOBAL INT LaserAxis ! Laser Control Module axis
GLOBAL INT Mode ! Modulation mode
GLOBAL INT Axes_Used ! Define which axes moves
GLOBAL REAL Freq, Width, Duty_Cycle ! PWM characteristics
STOP

&LaserRepeat_LCM_FIXED_FREQ
FIXED_FREQ_#AxisNo#:
    LaserAxis = #AxisNo# ! The last axis of the LCM module
    Mode = 1 ! Fixed frequency operation mode
    !Freq = 50000 ! Generated frequency of Hz
    !Width = 0.002 ! Generated pulse width us - not relevant in fixed frequency mode
    !Duty_Cycle = 0 ! Generated Duty Cycle of %
    Axes_Used = 0 ! Gives the ability to change Duty Cycle.
                  ! If Not used, PFGPAR can be directly updated by the application
                  !******************************************************************
                  ! Width Parameter is not relevant in fixed frequency mode.
                  ! By PFGPAR Duty Cycle may be changed.
                  !******************************************************************
    LCMODULATION ( LaserAxis, Mode, Freq, Width, Duty_Cycle, Axes_Used ) ! Initialization
    LCEnable (LaserAxis) ! Start genarate PWM
STOP

STOP_PULSES_#AxisNo#:
    LaserAxis = #AxisNo# ! The last axis of the LCM module
    LCStop(LaserAxis) ! Stops any previously initialized laser mode
    LCDisable (LaserAxis) ! Stop generate PWM
STOP


&