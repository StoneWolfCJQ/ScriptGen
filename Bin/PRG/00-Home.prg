!---------------------Home Script
REAL NSpeed, HSpeed
INT nAxis
REAL safePos
INT onLimit
INT ZAxisNo
REAL homeOffset
INT S_TEMP(200)

!---------------------Custom Functions Here
!!---------------SafeCheck: Z Goto RLimit   
SafeCheck:
&SafeBufferRepeat
	STOP #BufferNo#

&
&SafeBufferRepeat
	HALT #AxisNo#

&
&ZAxisSafeRepeat
	ZAxisNo=#AxisNo#
	CALL ZAxisSafe

&
&SafeBufferRepeat
	IF ^PST(#BufferNo#).#RUN
		Start #BufferNo#, 1
	END
&
	GLOBAL INT ZL@@BH(200)
	FILL(0, ZL@@BH, 0, 199)
	@@ZLimitSafeLine__WHILE (1)
&ZLimitSafeRepeat
		IF FAULT(#AxisNo#).#RL = 1
			ZL@BH(#AxisNo#) = 1
		END

&
	@@ZLimitSafeLine__	IF &ZLimitSafeRepeat
ZL@BH(#AxisNo#) = 1 $ 
&

	@@ZLimitSafeLine__		GOTO Main
	@@ZLimitSafeLine__	END
	@@ZLimitSafeLine__END

!---------------------Homing Process
!!---------------Some Encoders Are Absolute
!!---------------Don't Go Home After First Home	
Main:
!!---------------Axis Description

!--------------Axis Set And Process
&HomeRepeat
!#NAME#
SINGLE_AXIS_HOME(#AxisNo#).0 = 0
HomeSetAxis#AxisNo#:
nAxis = #AxisNo#
NSpeed = #NSpeed#
HSpeed = #HSpeed#
safePos = @HP
homeOffset = @HF
CALL AxisSet
!CALL LeaveLimit
CALL #HomingMethod#
#GoSafe#CALL GoSafePos!Optional
CALL HomeComplete
#COMP#CALL ErrorCompCallAxis#AxisNo#
CALL SingleAxisHomeStop


&
STOP

!!--------Axis Compensation
&CompRepeat
ErrorCompCallAxis#AxisNo#:
ERRORMAP1D nAxis, 0, @CS, @CT, ErrorCompData#NAME#
ERRORMAPON nAxis, 0
RET


&
!---------------------Home Functions
!!--------Parameter Setting
AxisSet:
	ERRORUNMAP nAxis, 0
	ERRORMAPOFF nAxis, 0
	ENABLE nAxis
	MFLAGS(nAxis).17 = 1
	VEL(nAxis)=NSpeed
	ACC(nAxis)=NSpeed*10
	DEC(nAxis)=NSpeed*10
	KDEC(nAxis)=NSpeed*30
	JERK(nAxis)=NSpeed*150

	FDEF(nAxis).#SLL=0
	FDEF(nAxis).#SRL=0
	MFLAGS(nAxis).#HOME=0

	S_TEMP(nAxis).0 = SINGLE_AXIS_HOME(nAxis).0
	SINGLE_AXIS_HOME(nAxis).0 = 0
RET

!!--------Away from Limit
LeaveLimit:
	IF (FAULT(nAxis).#LL = 1)
		JOG/v nAxis, NSpeed
		TILL ^FAULT(nAxis).#LL
		WAIT 200
		HALT nAxis
		TILL ^MST(nAxis).#MOVE
		WAIT 100
	END
	IF (FAULT(nAxis).#RL = 1)
		JOG/v nAxis, -NSpeed
		TILL ^FAULT(nAxis).#RL
		WAIT 200
		HALT nAxis
		TILL ^MST(nAxis).#MOVE
		WAIT 100
	END
RET

!!--------No Index Home
L:
	IF ^FAULT(nAxis).#LL
		JOG/v nAxis, -NSpeed
	END
	TILL FAULT(nAxis).#LL
	HALT nAxis
	TILL ^MST(nAxis).#MOVE

	JOG/v nAxis, NSpeed / 2
	TILL ^FAULT(nAxis).#LL
	WAIT 200
	HALT nAxis
	TILL ^MST(nAxis).#MOVE
	
	JOG/v nAxis, -HSpeed
	TILL FAULT(nAxis).#LL $ ^MST(nAxis).#MOVE
	WAIT 200
	PTP/er nAxis, homeOffset
	WAIT 200
	SET FPOS(nAxis) = 0
RET

!!--------No Index Home Reverse
R:
	IF ^FAULT(nAxis).#RL
		JOG/v nAxis, NSpeed
	END
	TILL FAULT(nAxis).#RL
	HALT nAxis
	WAIT 200

	JOG/v nAxis, -NSpeed / 2
	TILL ^FAULT(nAxis).#RL, 200
	WAIT 200
	HALT nAxis
	TILL ^MST(nAxis).#MOVE
	IF FAULT(nAxis).#RL
		RET
	END

	JOG/v nAxis, HSpeed
	TILL FAULT(nAxis).#RL $ ^MST(nAxis).#MOVE
	WAIT 200
	PTP/er nAxis, homeOffset
	WAIT 200
	SET FPOS(nAxis) = 0
RET

!!--------Index Home
LI:
	IF ^FAULT(nAxis).#LL
		JOG/v nAxis, -NSpeed
	END
	TILL FAULT(nAxis).#LL $ ^MST(nAxis).#MOVE
	WAIT 200

	IST(nAxis).#IND=0
	WAIT 50
	JOG/v nAxis, HSpeed
	TILL IST(nAxis).#IND=1
	HALT nAxis

	CALL CheckIFOnLimit
	IF onLimit=1
		PTP/r nAxis, 20
		IST(nAxis).#IND=0
		WAIT 50
		JOG/v nAxis, HSpeed
		TILL IST(nAxis).#IND=1
		HALT nAxis
	END
	WAIT 100
	SET FPOS(nAxis)=FPOS(nAxis)-IND(nAxis) + homeOffset
	WAIT 100
	PTP nAxis, homeOffset
RET

!!--------Index Home Reverse
RI:
	IF ^FAULT(nAxis).#RL
		JOG/v nAxis, NSpeed
	END
	TILL FAULT(nAxis).#RL $ ^MST(nAxis).#MOVE
	WAIT 200

	IST(nAxis).#IND=0
	WAIT 50
	JOG/v nAxis,-HSpeed
	TILL IST(nAxis).#IND=1
	HALT nAxis

	CALL CheckIFOnLimit
	IF onLimit=1
		PTP/r nAxis, -20
		IST(nAxis).#IND=0
		WAIT 50
		JOG/v nAxis, -HSpeed
		TILL IST(nAxis).#IND=1
		HALT nAxis
	END
	WAIT 100
	SET FPOS(nAxis)=FPOS(nAxis)-IND(nAxis) + homeOffset
	WAIT 100
	PTP nAxis, homeOffset
RET

!!--------Check IF Index Near Limit
CheckIFOnLimit:
	onLimit=0
	IF FAULT(nAxis).#LL | FAULT(nAxis).#RL
		onLimit=1
	END
RET

!!--------SafePos
GoSafePos:
	PTP/e nAxis,safePos
RET

!!--------Z Axis Go Right
ZAxisSafe:
	ENABLE (ZAxisNo)
	FDEF(ZAxisNo).#SLL=0
	FDEF(ZAxisNo).#SRL=0
	JOG/v ZAxisNo, 10
RET

!!--------Home Complete Action
HomeComplete:
	MFLAGS(nAxis).#HOME = 1
	FDEF(nAxis).#SLL=1
	FDEF(nAxis).#SRL=1
RET

!!--------Single Axis Home Func
SingleAxisHomeStop:
	IF S_TEMP(nAxis).0
		STOP
	ELSE
		RET
	END