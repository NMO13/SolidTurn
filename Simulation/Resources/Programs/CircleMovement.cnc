;rough part length: 450 radius part radius: 40
;Tool slots
;TOOL = (0000 ,Neutral   )
;TOOL = (0001 ,Left-Thin  )
;TOOL = (0002 ,Right-Thin   )
;TOOL = (0003, Cut-Off)
;TOOL = (0004, Left)

N0005 S200 T0001 F2000
N0 G92 X0 Z410
N0060 M4
N30 G0 X60. Z5.
N30 G0 X20. Z0.
N30 G3 X40. Z-20 I0. K-20
N30 G0 X60. Z-50.
N30 T0000
N30 G0 X40 Z-20
N30 G3 X20. Z-40 I-20. K0
N40 M5
N40 M30