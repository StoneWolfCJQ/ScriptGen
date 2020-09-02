!---------------------Compensating Script
!!---------------Data Program
AUTOEXEC:
SINGLE_AXIS_HOME_NO = -1
&CompRepeat
AxisComp#NAME#:
!---DataArea
!ErrorCompData#NAME#(0)=...
!...
!---DataArea       


&  
STOP

!!---------------Single Axis Home
SingleAxisHome:
IF SINGLE_AXIS_HOME_NO < 0 | SINGLE_AXIS_HOME_NO > 199
    STOP
END

&SingleAxisHome
!#NAME#
IF SINGLE_AXIS_HOME_NO = #AxisNo# $ ^PST(@HG).#RUN
    SINGLE_AXIS_HOME(SINGLE_AXIS_HOME_NO).0 = 1
    START @HG, HomeSetAxis#AxisNo#
    WHILE PST(@HG).#RUN
        WAIT 5
    END
END

&

SINGLE_AXIS_HOME_NO = -1
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

INT _sign
_sign = (endPos - startPos) / ABS(endPos - startPos)
step = ABS(step) * _sign
IF (step = 0)
    STOP
END

IF FPOS(axisNo) <> startPos
    PTP axisNo, startPos
    TILL ^MST(axisNo).#MOVE
    IF MST(axisNo).0 <> 1
        STOP
    END
END

!Motion process
INT s;s=startPos
LOOP 2
    WHILE(_sign * s < _sign * endPos)
        WAIT dwell
        s=s+step
        PTP axisNo,s
        TILL ^MST(axisNo).#MOVE
    END

    WHILE(_sign * s > _sign * startPos)
        WAIT dwell
        s=s-step
        PTP axisNo,s
        TILL ^MST(axisNo).#MOVE
    END
END
STOP