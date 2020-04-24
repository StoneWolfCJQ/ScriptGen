!---------------------AutoExecute Script
!AUTOEXEC:
INT AxisNo

!!---------------Input And Ouput Slave Index
IOConfig:
&IOINRepeat
	ECIN(ECGETOFFSET("Input", #SlaveIndex#, 1), @INAME#IOIndexA#)
	ECIN(ECGETOFFSET("Input", #SlaveIndex#, 1)+1, @INAME#IOIndexB#)

&

&IOOUTRepeat
	ECOUT(ECGETOFFSET("Output", #SlaveIndex#, 0), @ONAME#IOIndexA#)
	ECOUT(ECGETOFFSET("Output", #SlaveIndex#, 0)+1, @ONAME#IOIndexB#)

&

!!---------------Axis No, Node And Actual Pos
AxisConfig:
&CANRepeat
	AxisNo=#AxisNo#
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
@LIGHT=1
Unstop:
	WHILE 1		
        EMG=9
		!EMG Function
		IF @@EMG=0
			KILL ALL		
			@MOTOR=0
			@DUST=0
			@SHUTTER=0
			@RED=1
			@BUZZ=1
			@DOOR=0
			@ION=0
		    HALT ALL
			DISABLE ALL
			EONCE=0
		ELSE
			IF EONCE=0
				@RED=0
				@BUZZ=0
				EONCE=1
			END
		END
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
		ERRORUNMAP AxisNo,0
		ENABLE AxisNo
		WAIT 2000
		COMMUT AxisNo
		WAIT 2000
		DISABLE AxisNo
		WAIT 500
    END


&
RET

!!---------------Limit Detecting
LimitDetect:
&LimitRepeat
    AxisNo=#AxisNo#
#RightLimit#	IF @RL
#RightLimit#		SAFINI(AxisNo).#RL=1
#RightLimit#	ELSE
#RightLimit#		SAFINI(AxisNo).#RL=0
#RightLimit#	END
#LeftLimit#	IF @LL
#LeftLimit#		SAFINI(AxisNo).#LL=1
#LeftLimit#	ELSE
#LeftLimit#		SAFINI(AxisNo).#LL=0
#LeftLimit#	END	


&
RET

!!---------------For Safety
SafeGuard:
    !Optional: Set Speed
RET