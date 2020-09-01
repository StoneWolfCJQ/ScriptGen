!---------------------AutoExecute Script
!AUTOEXEC:
INT AxisNo, _sum, _limit_L, _limit_R

!!---------------Input And Ouput Slave Index
!!@MapCommand: ECIN or ECOUT; IOType: I-1, O-0
IOConfig:
	INT IOStartIndex
&IORepeat
	IOStartIndex = #StartIndex#
	#MapCommand#(ECGETOFFSET("#MappingName#", IOStartIndex + #Increment#, #IOType#) + #NUM#, @IONAME#IOIndex#)

&

!!---------------Axis No, Node And Actual Pos
AxisConfig:
&CANRepeat
	AxisNo=#AxisNo#
	DISABLE AxisNo
	EFAC(AxisNo)=@SD/@PS
	MFLAGS(AxisNo).#INVENC=@INV		
	TARGRAD(AxisNo)=@TARGRAD
	SETTLE(AxisNo)=@SETTLE
	CERRI(AxisNo)=@CERRI
	CERRV(AxisNo)=@CERRV
	CERRA(AxisNo)=@CERRA
	XVEL(AxisNo)=@XVEL
	XACC(AxisNo)=@XVEL*10


&

!!---------------Commutation Process
Call CommutProcess

!!---------------While Loop
INT EONCE;EONCE=1
INT EMG
INT COUNT;COUNT=0
@LIGHT=1
Unstop:
	WHILE 1		
		CALL EMGProtect
		CALL LimitDetect
		CALL SafeGuard 
	END
STOP

!!---------------Commutation Process
CommutProcess:  
    @MOTOR=1
	WAIT 3000

&CommutRepeat
    AxisNo=#AxisNo#
	IF ^MFLAGS(AxisNo).#BRUSHOK
		MFLAGS(AxisNo).17 = 1
		ENABLE AxisNo
		WAIT 2000
		COMMUT AxisNo
		WAIT 2000
		DISABLE AxisNo
		WAIT 500
    END


&
RET

!!---------------EMG Protection
EMGProtect:
	EMG=9
	!EMG Function
	IF @@EMG=0
		COUNT=COUNT+1
		KILL ALL		
		@MOTOR=0!MOTOR
		@DUST=0!DUST
		@SHUTTER=0!SHUTTER
		@RED=1!RED
		@GREEN=0!GREEN
		@DOOR=0!DOOR
		@ION=0!ION
		IF COUNT = 1
			@BUZZ=1!BUZZ
		END	
		HALT ALL
		DISABLE ALL
		EONCE=0
	ELSE
		IF EONCE=0
			@RED=0!RED
			@BUZZ=0!BUZZ
			EONCE=1
			COUNT=0
		END
	END
RET

!!---------------Limit Detecting
LimitDetect:
&LimitRepeat
    AxisNo = #AxisNo#
#RightLimit#	_limit_R = @RL
#LeftLimit#	_limit_L = @LL
#RightLimit#	CALL RightLimitA
#LeftLimit#	CALL LeftLimitA
!#RightLimit#	IF @RL
!#RightLimit#		SAFINI(AxisNo).#RL=1
!#RightLimit#	ELSE
!#RightLimit#		SAFINI(AxisNo).#RL=0
!#RightLimit#	END
!#LeftLimit#	IF @LL
!#LeftLimit#		SAFINI(AxisNo).#LL=1
!#LeftLimit#	ELSE
!#LeftLimit#		SAFINI(AxisNo).#LL=0
!#LeftLimit#	END	


&
RET

!!---------------For Safety
SafeGuard:
&SAFE_MACRO
#ITEM#
&
RET

!!---------------Right Limit Algorithm
RightLimitA:
	IF _limit_R > 1 | _limit_R < 0
		_limit_R = 0
	END
	_sum = 4*_limit_R + 2*FAULT(AxisNo).#RL + SAFINI(AxisNo).#RL
	IF _sum = 0 | _sum = 3 | _sum = 5 | _sum = 6
		SAFINI(AxisNo).#RL = 1
	ELSE
		SAFINI(AxisNo).#RL = 0
	END
RET

!!---------------Left Limit Algorithm
LeftLimitA:
	IF _limit_L > 1 | _limit_L < 0
		_limit_L = 0
	END
	_sum = 4*_limit_L + 2*FAULT(AxisNo).#LL + SAFINI(AxisNo).#LL
	IF _sum = 0 | _sum = 3 | _sum = 5 | _sum = 6
		SAFINI(AxisNo).#LL = 1
	ELSE
		SAFINI(AxisNo).#LL = 0
	END
RET