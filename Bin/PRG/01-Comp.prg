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
INT dwell, endPos, vel
INT errCompOn
GLOBAL INT axisNo, startPos, step

axisNo=0
startPos=0
endPos=0
step=0
vel=30
errCompOn=0
dwell=6000

IF errCompOn=1
    !Please run the script from line 1 after data pasting and before mapping
    MFLAGS(axisNo).17 = 0
&CompRepeat
    IF axisNo=#AxisNo#
        CONNECT RPOS(axisNo) = APOS(axisNo) + MAP(APOS(axisNo), ErrorCompData#NAME#, startPos, step) 
    END

&
    DEPENDS axisNo, axisNo
ELSE
    MFLAGS(axisNo).17 = 1
END

!Motion parameters
ENABLE axisNo
VEL(axisNo)=vel
ACC(axisNo)=vel*10
DEC(axisNo)=vel*10
KDEC(axisNo)=vel*20
JERK(axisNo)=vel*50

!Motion process
INT s;s=startPos
LOOP 2
    WHILE(s < endPos)
        WAIT dwell
        s=s+step
        PTP axisNo,s
        TILL ^MST(axisNo).#MOVE
    END

    WHILE(s > startPos)
        WAIT dwell
        s=s-step
        PTP axisNo,s
        TILL ^MST(axisNo).#MOVE
    END
END
STOP