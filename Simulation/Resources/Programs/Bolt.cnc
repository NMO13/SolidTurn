;rough part length: 250 radius part radius: 40
;Tool slots
;TOOL = (0000 ,Neutral   )
;TOOL = (0001 ,Left-Thin  )
;TOOL = (0002 ,Right-Thin   )
;TOOL = (0003, Cut-Off)
;TOOL = (0004, Left)

N30 S200 T0001 F2000
N30 G92 X0 Z210
N30 M4
N30 G0 X32.2188 Z0.
N30 G0 X32.2188 Z-140
N30 G0 X41 Z-140
N30 G0 X41 Z0
N30 G0 X24.2188 Z0
N30 G0 X24.2188 Z-63.8
N30 G0 X35 Z-63.8
N30 G0 X35 Z0
N30 G0 X19 Z0
N30 G0 X19 Z-40
N30 G0 X36 Z-40
N30 G0 X36 Z0
N30 G0 X10 Z0
N30 G0 X10 Z-40
N30 G0 X36 Z-40
N30 G0 X36 Z0
N30 G0 X0 Z0
N30 G3 X10 Z-10 I0 K-10
N30 G0 X10 Z-40
N30 G2 X20 Z-50 I10 K0
N30 G0 X24.2188 Z-50
N30 G0 X24.2188 Z-63.8
N30 G2 X29.2188 Z-68.8 I5 K0
N30 G0 X32.2188 Z-71.8
N30 G0 X60. Z150.
N30 T0000
N30 G0 X40 Z-81.8
N30 G0 X32.2188 Z-81.8
N30 G0 X29.2188 Z-84.8
N30 G0 X29.2188 Z-104
N30 G0 X20 Z-140
N30 G0 X30 Z-140
N30 G0 X30 Z-104
N30 G0 X16.2 Z-104
N30 G0 X16.2 Z-140
N30 G0 X40 Z-140
N30 G0 X60. Z150.
N30 T0003
N30 G0 X40. Z-140
N30 G0 X0. Z-140
N40 M5
N40 M30