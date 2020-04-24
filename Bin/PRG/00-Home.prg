!---------------------Home Script
REAL dfSpeed
INT nAxis
REAL safePos
INT onLimit
INT ZAxisNo
!!---------------Index folloing para
REAL homeOffset

!---------------------Custom Functions Here
!!---------------SafeCheck: Z Goto RLimit   
SafeCheck:
	HALT #ALL#
&SafeRepeat
	STOP #BufferNo#

&
&ZAxisSafeRepeat
	ZAxisNo=#AxisNo#
	CALL ZAxisSafe

&
&BufferRepeat
	IF ^PST(#BufferNo#).#RUN
		Start #BufferNo#, 1
	END
&


!---------------------Homing Process
!!---------------Some Encoders Are Absolute
!!---------------Don't Go Home After First Home
Main:
!!---------------Axis Description

!--------------Axis Set And Process
&HomeRepeat
nAxis=#AxisNo#
dfSpeed=@HS
safePos=@HP
homeOffset=@HF
call AxisSet
call LeaveLimit
call #HomingMethod#
#GoSafe#call GoSafePos!Optional


&
&CompRepeat
TILL ^PST(#BCNo#).#RUN
Start #BCNo#, AxisComp#AxisNo#
WAIT 100


&
STOP
!---------------------Home Functions
!!--------Parameter Setting
AxisSet:
	ENABLE nAxis
	ERRORUNMAP nAxis, 0
	VEL(nAxis)=dfSpeed
	ACC(nAxis)=dfSpeed*10
	DEC(nAxis)=dfSpeed*10
	KDEC(nAxis)=dfSpeed*30
	JERK(nAxis)=dfSpeed*150
RET

!!--------Away from Limit
LeaveLimit:
	IF (FAULT(nAxis).#LL = 1)
		JOG/v nAxis,dfSpeed/2
		TILL ^FAULT(nAxis).#LL
		WAIT 200
		HALT nAxis
		TILL ^MST(nAxis).#MOVE
		WAIT 100
	END
	IF (FAULT(nAxis).#RL = 1)
		JOG/v nAxis,-dfSpeed/2
		TILL ^FAULT(nAxis).#RL
		WAIT 200
		HALT nAxis
		TILL ^MST(nAxis).#MOVE
		WAIT 100
	END
RET

!!--------No Index Home
L:
	JOG/v nAxis,-dfSpeed
	TILL FAULT(nAxis).#LL&^MST(nAxis).#MOVE
	WAIT 200
	PTP/er nAxis,2
	WAIT 200
	SET FPOS(nAxis)= 0
	WAIT 100
RET

!!--------No Index Home Reverse
R:
	JOG/v nAxis,dfSpeed
	TILL FAULT(nAxis).#RL&^MST(nAxis).#MOVE
	WAIT 200
	PTP/er nAxis,-2
	WAIT 200
	SET FPOS(nAxis)= 0
	WAIT 100
RET

!!--------Index Home
LI:
	JOG/v nAxis,-dfSpeed
	TILL FAULT(nAxis).#LL&^MST(nAxis).#MOVE
	WAIT 200

	IST(nAxis).#IND=0
	WAIT 50
	JOG/v nAxis,dfSpeed/2
	TILL IST(nAxis).#IND=1
	HALT nAxis

	CALL CheckIFOnLimit
	IF onLimit=1
		PTP/r nAxis,20
		IST(nAxis).#IND=0
		WAIT 50
		JOG/v nAxis,dfSpeed/2
		TILL IST(nAxis).#IND=1
		HALT nAxis
	END
	WAIT 100
	SET FPOS(nAxis)=FPOS(nAxis)-IND(nAxis) + homeOffset
RET

!!--------Index Home Reverse
RI:
	JOG/v nAxis,dfSpeed
	TILL FAULT(nAxis).#RL&^MST(nAxis).#MOVE
	HALT nAxis
	WAIT 200

	IST(nAxis).#IND=0
	WAIT 50
	JOG/v nAxis,-dfSpeed/2
	TILL IST(nAxis).#IND=1
	HALT nAxis

	CALL CheckIFOnLimit
	IF onLimit=1
		PTP/r nAxis,-20
		IST(nAxis).#IND=0
		WAIT 50
		JOG/v nAxis,-dfSpeed/2
		TILL IST(nAxis).#IND=1
		HALT nAxis
	END
	WAIT 100
	SET FPOS(nAxis)=FPOS(nAxis)-IND(nAxis) + homeOffset
RET

!!--------Check IF Index Near Limit
CheckIFOnLimit:
	onLimit=0
	IF FAULT(nAxis).#LL|FAULT(nAxis).#RL
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
	IF ^FAULT(ZAxisNo).#RL
		JOG/v ZAxisNo, 10
		TILL FAULT(ZAxisNo).#RL
	END
RET