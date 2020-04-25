#/ Controller version = 2.70
#/ Date = 04/25/2020 10:28 AM
#/ User remarks = Automatic generated by ScriptGen

#3
!ForSearchUseDoNotDuplicate
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


!---------------------Homing Process
!!---------------Some Encoders Are Absolute
!!---------------Don't Go Home After First Home
Main:
!!---------------Axis Description

!--------------Axis Set And Process
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

#2
!ForSearchUseDoNotDuplicate
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


!---------------------Homing Process
!!---------------Some Encoders Are Absolute
!!---------------Don't Go Home After First Home
Main:
!!---------------Axis Description

!--------------Axis Set And Process
nAxis=4
dfSpeed=20
safePos=30
homeOffset=0
call AxisSet
call LeaveLimit
call LI
call GoSafePos!Optional

nAxis=5
dfSpeed=20
safePos=20
homeOffset=0
call AxisSet
call LeaveLimit
call LI
call GoSafePos!Optional

nAxis=6
dfSpeed=20
safePos=30
homeOffset=0
call AxisSet
call LeaveLimit
call LI
call GoSafePos!Optional

nAxis=7
dfSpeed=20
safePos=20
homeOffset=0
call AxisSet
call LeaveLimit
call LI
call GoSafePos!Optional

TILL ^PST(5).#RUN
Start 5, AxisComp4
WAIT 100

TILL ^PST(5).#RUN
Start 5, AxisComp5
WAIT 100

TILL ^PST(5).#RUN
Start 5, AxisComp6
WAIT 100

TILL ^PST(5).#RUN
Start 5, AxisComp7
WAIT 100

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

#4
!ForSearchUseDoNotDuplicate
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
	ZAxisNo=8
	CALL ZAxisSafe
	ZAxisNo=9
	CALL ZAxisSafe


!---------------------Homing Process
!!---------------Some Encoders Are Absolute
!!---------------Don't Go Home After First Home
Main:
!!---------------Axis Description

!--------------Axis Set And Process
nAxis=8
dfSpeed=50
safePos=0
homeOffset=0
call AxisSet
call LeaveLimit
call R
!call GoSafePos!Optional

nAxis=9
dfSpeed=50
safePos=0
homeOffset=0
call AxisSet
call LeaveLimit
call R
!call GoSafePos!Optional

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

#10
!ForSearchUseDoNotDuplicate
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


!---------------------Homing Process
!!---------------Some Encoders Are Absolute
!!---------------Don't Go Home After First Home
Main:
!!---------------Axis Description

!--------------Axis Set And Process
nAxis=0
dfSpeed=50
safePos=0
homeOffset=0
call AxisSet
call LeaveLimit
call L
!call GoSafePos!Optional

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

#11
!ForSearchUseDoNotDuplicate
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


!---------------------Homing Process
!!---------------Some Encoders Are Absolute
!!---------------Don't Go Home After First Home
Main:
!!---------------Axis Description

!--------------Axis Set And Process
nAxis=1
dfSpeed=50
safePos=0
homeOffset=0
call AxisSet
call LeaveLimit
call R
!call GoSafePos!Optional

nAxis=2
dfSpeed=50
safePos=0
homeOffset=0
call AxisSet
call LeaveLimit
call R
!call GoSafePos!Optional

nAxis=3
dfSpeed=50
safePos=0
homeOffset=0
call AxisSet
call LeaveLimit
call R
!call GoSafePos!Optional

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

#5
!ForSearchUseDoNotDuplicate
!---------------------Compensating Script
AxisComp4:
ERRORUNMAP 4, 0
!---DataArea

!---DataArea         
ERRORMAP1D 4, 0, 0, 30, ErrorCompDataAxis4
ERRORMAPON 4, 0
STOP

AxisComp5:
ERRORUNMAP 5, 0
!---DataArea

!---DataArea         
ERRORMAP1D 5, 0, 0, 30, ErrorCompDataAxis5
ERRORMAPON 5, 0
STOP

AxisComp6:
ERRORUNMAP 6, 0
!---DataArea

!---DataArea         
ERRORMAP1D 6, 0, 0, 30, ErrorCompDataAxis6
ERRORMAPON 6, 0
STOP

AxisComp7:
ERRORUNMAP 7, 0
!---DataArea

!---DataArea         
ERRORMAP1D 7, 0, 0, 30, ErrorCompDataAxis7
ERRORMAPON 7, 0
STOP


#6
!ForSearchUseDoNotDuplicate


#31
!ForSearchUseDoNotDuplicate
!---------------------AutoExecute Script
!AUTOEXEC:
INT AxisNo

!!---------------Input And Ouput Slave Index
IOConfig:
	ECIN(ECGETOFFSET("Input", 6, 1), EIN5)
	ECIN(ECGETOFFSET("Input", 6, 1)+1, EIN6)
	ECIN(ECGETOFFSET("Input", 13, 1), EIN7)
	ECIN(ECGETOFFSET("Input", 13, 1)+1, EIN8)
	ECIN(ECGETOFFSET("Input", 14, 1), EIN9)
	ECIN(ECGETOFFSET("Input", 14, 1)+1, EIN10)
	ECIN(ECGETOFFSET("Input", 16, 1), EIN11)
	ECIN(ECGETOFFSET("Input", 16, 1)+1, EIN12)
	ECIN(ECGETOFFSET("Input", 17, 1), EIN13)
	ECIN(ECGETOFFSET("Input", 17, 1)+1, EIN14)

	ECOUT(ECGETOFFSET("Output", 7, 0), EOUT7)
	ECOUT(ECGETOFFSET("Output", 7, 0)+1, EOUT8)
	ECOUT(ECGETOFFSET("Output", 8, 0), EOUT9)
	ECOUT(ECGETOFFSET("Output", 8, 0)+1, EOUT10)
	ECOUT(ECGETOFFSET("Output", 9, 0), EOUT11)
	ECOUT(ECGETOFFSET("Output", 9, 0)+1, EOUT12)
	ECOUT(ECGETOFFSET("Output", 10, 0), EOUT13)
	ECOUT(ECGETOFFSET("Output", 10, 0)+1, EOUT14)
	ECOUT(ECGETOFFSET("Output", 11, 0), EOUT15)
	ECOUT(ECGETOFFSET("Output", 11, 0)+1, EOUT16)

!!---------------Axis No, Node And Actual Pos
AxisConfig:
	AxisNo=8
	EFAC(AxisNo)=5/8388608
	MFLAGS(AxisNo).#INVENC=0		
	TARGRAD(AxisNo)=0.0005
	SETTLE(AxisNo)=5
	CERRI(AxisNo)=5
	CERRV(AxisNo)=5
	CERRA(AxisNo)=5
	XVEL(AxisNo)=500
	XACC(AxisNo)=500*10

	AxisNo=9
	EFAC(AxisNo)=5/8388608
	MFLAGS(AxisNo).#INVENC=0		
	TARGRAD(AxisNo)=0.0005
	SETTLE(AxisNo)=5
	CERRI(AxisNo)=5
	CERRV(AxisNo)=5
	CERRA(AxisNo)=5
	XVEL(AxisNo)=500
	XACC(AxisNo)=500*10


!!---------------Commutation Process
Call CommutProcess

!!---------------While Loop
INT EONCE;EONCE=1
INT EMG
EIN0.3=1
Unstop:
	WHILE 1		
        EMG=9
		!EMG Function
		IF EMG=0
			KILL ALL		
			EIN0.5=0
			EIN3.0=1
		    HALT ALL
			DISABLE ALL
			EONCE=0
		ELSE
			IF EONCE=0
				EIN3.0=0
				EONCE=1
			END
		END
		CALL LimitDetect
		CALL SafeGuard 
	END
STOP

!!---------------Commutation Process
CommutProcess:  
    EIN0.5=1
	WAIT 3000

    AxisNo=4
	IF ^MFLAGS(AxisNo).#BRUSHOK
		ERRORUNMAP AxisNo,0
		ENABLE AxisNo
		WAIT 2000
		COMMUT AxisNo
		WAIT 2000
		DISABLE AxisNo
		WAIT 500
    END

    AxisNo=5
	IF ^MFLAGS(AxisNo).#BRUSHOK
		ERRORUNMAP AxisNo,0
		ENABLE AxisNo
		WAIT 2000
		COMMUT AxisNo
		WAIT 2000
		DISABLE AxisNo
		WAIT 500
    END

    AxisNo=6
	IF ^MFLAGS(AxisNo).#BRUSHOK
		ERRORUNMAP AxisNo,0
		ENABLE AxisNo
		WAIT 2000
		COMMUT AxisNo
		WAIT 2000
		DISABLE AxisNo
		WAIT 500
    END

    AxisNo=7
	IF ^MFLAGS(AxisNo).#BRUSHOK
		ERRORUNMAP AxisNo,0
		ENABLE AxisNo
		WAIT 2000
		COMMUT AxisNo
		WAIT 2000
		DISABLE AxisNo
		WAIT 500
    END

RET

!!---------------Limit Detecting
LimitDetect:
    AxisNo=8
!	IF #RL
!		SAFINI(AxisNo).#RL=1
!	ELSE
!		SAFINI(AxisNo).#RL=0
!	END
	IF EIN0.1
		SAFINI(AxisNo).#LL=1
	ELSE
		SAFINI(AxisNo).#LL=0
	END	

    AxisNo=9
!	IF #RL
!		SAFINI(AxisNo).#RL=1
!	ELSE
!		SAFINI(AxisNo).#RL=0
!	END
	IF EIN0.4
		SAFINI(AxisNo).#LL=1
	ELSE
		SAFINI(AxisNo).#LL=0
	END	

RET

!!---------------For Safety
SafeGuard:
    !Optional: Set Speed
RET

#A
!ForSearchUseDoNotDuplicate
GLOBAL STATIC REAL ErrorCompDataAxis4(100),ErrorCompDataAxis5(100),ErrorCompDataAxis6(100),ErrorCompDataAxis7(100)

GLOBAL INT EIN5, EIN6, EIN7, EIN8, EIN9, EIN10, EIN11, EIN12, EIN13, EIN14 

GLOBAL INT EOUT7, EOUT8, EOUT9, EOUT10, EOUT11, EOUT12, EOUT13, EOUT14, EOUT15, EOUT16 
