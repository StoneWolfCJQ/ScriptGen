!---------------------Compensating Script
!!---------------Data Program
AUTOEXEC:
&CompRepeat
AxisComp#NAME#:
!---DataArea
!ErrorCompData#NAME#(0)=...
!...
!---DataArea       


&  
STOP

!!---------------Measure Program
CompMeasure:
INT axisNo, dwell, startPos,endPos,step,vel
INT errCompOn

axisNo=0
startPos=0
endPos=0
step=0
vel=30
errCompOn=0
dwell=6000

IF errCompOn=1
    !Please run the script from line 1 after data pasting and before mapping
    ERRORUNMAP axisNo, 0
&CompRepeat
    IF axisNo=#AxisNo#
        ERRORMAP1D axisNo, 0, startPos, step, ErrorCompData#NAME#
    END

&
    ERRORMAPON axisNo, 0
ELSE
    ERRORUNMAP axisNo, 0
END

!Motion parameters
VEL(axisNo)=vel
ACC(axisNo)=vel*10
DEC(axisNo)=vel*10
KDEC(axisNo)=vel*20
JERK(axisNo)=vel*50

!Motion process
INT s;s=startPos
LOOP 2
    WHILE(s<=endPos)
        PTP axisNo,s
        TILL ^MST(axisNo).#MOVE
        WAIT dwell
        s=s+step
    END

    WHILE(s>=startPos)
        PTP axisNo,s
        TILL ^MST(axisNo).#MOVE
        WAIT dwell
        s=s-step
    END
END
STOP