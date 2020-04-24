!---------------------Compensating Script
&CompRepeat
AxisComp#AxisNo#:
ERRORUNMAP #AxisNo#, 0
!---DataArea

!---DataArea         
ERRORMAP1D #AxisNo#, 0, @CS, @CT, ErrorCompDataAxis#AxisNo#
ERRORMAPON #AxisNo#, 0
STOP


&