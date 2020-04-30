!---------------------Home Script
REAL dfSpeed
INT nAxis
REAL safePos
INT onLimit
INT ZAxisNo
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

&HomeRepeat
nAxis=#AxisNo#
dfSpeed=@HS
safePos=@HP
homeOffset=@HF
call AxisSet
call LeaveLimit
call #HomingMethod#


&
&HomeRepeat
#GoSafe#call GoSafePos!Optional

&

&CompRepeat
TILL MFLAGS#AxisNo#.HOME = 1
ERRORMAP1D #AxisNo#, 0, @CS, @CT, ErrorCompData#NAME#
ERRORMAPON #AxisNo#, 0


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
	HOME nAxis, 17, dfSpeed, , homeOffset
RET

!!--------No Index Home Reverse
R:
	HOME nAxis, 18, dfSpeed, , homeOffset
RET

!!--------Index Home
LI:
	HOME nAxis, 1, dfSpeed, , homeOffset
RET

!!--------Index Home Reverse
RI:
	HOME nAxis, 2, dfSpeed, , homeOffset
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