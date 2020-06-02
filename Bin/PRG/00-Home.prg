!---------------------Home Script
REAL NSpeed, HSpeed
INT nAxis
REAL safePos
INT onLimit
INT ZAxisNo
REAL homeOffset

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
nAxis = #AxisNo#
NSpeed = #NSpeed#
HSpeed = #HSpeed#
safePos = @HP
homeOffset = @HF
CALL AxisSet
!CALL LeaveLimit
CALL #HomingMethod#
#GoSafe#CALL GoSafePos!Optional
MFLAGS(nAxis).#HOME = 1


&
&CompRepeat    
ERRORMAP1D #AxisNo#, 0, @CS, @CT, ErrorCompData#NAME#
ERRORMAPON #AxisNo#, 0


&
STOP
!---------------------Home Functions
!!--------Parameter Setting
AxisSet:
	ENABLE nAxis
	ERRORUNMAP nAxis, 0
	VEL(nAxis)=NSpeed
	ACC(nAxis)=NSpeed*10
	DEC(nAxis)=NSpeed*10
	KDEC(nAxis)=NSpeed*30
	JERK(nAxis)=NSpeed*150

	FDEF(nAxis).#SLL=0
	FDEF(nAxis).#SRL=0
	MFLAGS(nAxis).#HOME=0
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

	JOG/v nAxis, NSpeed
	TILL ^FAULT(nAxis).#LL
	WAIT 200
	HALT nAxis
	TILL ^MST(nAxis).#MOVE
	WAIT 100
	
	JOG/v nAxis, -HSpeed
	TILL FAULT(nAxis).#LL $ ^MST(nAxis).#MOVE
	WAIT 200
	PTP/er nAxis, homeOffset
	WAIT 200
	SET FPOS(nAxis) = 0
	WAIT 100
RET

!!--------No Index Home Reverse
R:
	IF ^FAULT(nAxis).#RL
		JOG/v nAxis, NSpeed
	END
	TILL FAULT(nAxis).#RL
	HALT nAxis
	WAIT 200

	JOG/v nAxis, -NSpeed
	TILL ^FAULT(nAxis).#RL
	WAIT 200
	HALT nAxis
	TILL ^MST(nAxis).#MOVE
	WAIT 100

	JOG/v nAxis, HSpeed
	TILL FAULT(nAxis).#RL $ ^MST(nAxis).#MOVE
	WAIT 200
	PTP/er nAxis, homeOffset
	WAIT 200
	SET FPOS(nAxis) = 0
	WAIT 100
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
	JOG/v ZAxisNo, 10
RET