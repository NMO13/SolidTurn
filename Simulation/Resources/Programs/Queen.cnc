;rough part length: 380 radius part radius: 40
;TOOL = (0000 ,Neutral)
;TOOL = (0001 ,Left-Thin)
;TOOL = (0002 ,Right-Thin)
;TOOL = (0003, Cut-Off)
;TOOL = (0004, Left)

N0005 S200 T0001 F2000
N0 G92 X0 Z340
N30 M3
N30 G0 X35. Z0.
N30 G0 X35. Z-243.0
N30 G0 X42. Z-243.0
N30 G0 X42. Z-114.0
N30 G0 X23. Z-114.0
N30 G0 X23. Z-186.0
N30 G0 X42. Z-186.0
N30 G0 X42. Z5.
N30 G0 X30. Z5.
N30 G0 X30. Z-59.8061
N30 G0 X45. Z-59.8061
N30 G0 X45. Z0.
N30 G0 X25.6199 Z0.
N30 G0 X25.6199 Z-59.8061
N30 G0 X30. Z-59.8061
N30 G0 X30 Z-34.
N30 G0 X20 Z-34.
N30 G0 X20 Z-53.
N30 G0 X40 Z-53.
N30 G0 X40 Z0.
N30 G0 X15 Z0.
N30 G0 X15 Z-10.0069
N30 G0 X30 Z-10.0069
N30 G0 X30 Z5

N30 G0 X0 Z0.
N30 G3 X10.0069 Z-10.0069 I0. K-10.0069
N30 G0 X18.5422 Z-10.0069
N30 G0 X25.6199 Z-22.6747
N30 G0 X25.6199 Z-59.8601
N30 G0 X42. Z-59.8601
N30 G0 X60. Z20.
N30 T0000
N30 G0 X25.6199 Z-22.6747
N30 G2 X18.0547 Z-54.7341 I4.1917 K-17.9115
N30 G0 X42. Z-54.7341
N30 G0 X60. Z20.
N30 T0001
N30 G0 X18.0547 Z-54.7341
N30 G0 X18.0547 Z-59.8061
N30 G0 X40.8365 Z-59.8061
N30 G0 X60. Z20.
N30 T0000
N30 G0 X29.8365 Z-59.8061
N30 G3 X30 Z-75.8587 I-6.1345 K-8.0896
N30 G0 X30 Z-83.4416
N30 G2 X29.7555 Z-207.6977 I127.2245 K-62.3786
N30 G0 X29.7555 Z-209.5072
N30 G3 X29.5039 Z-212.6529 I-1.3149 K-1.4777
N30 G0 X35 Z-212.6529
N30 G0 X60. Z20.
N30 T0004
N30 G0 X35 Z-212.6529
N30 G0 X29.5039 Z-212.6529
N30 G0 X29.5039 Z-219.9416
N30 G0 X35. Z-219.9416
N30 G0 X60. Z20.
N30 T0003
N30 G0 X35. Z-243.0
N30 G0 X0. Z-243.0
N00 M5
N30 M30