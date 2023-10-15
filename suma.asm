; Autor: Guillermo Fernandez Romero
; Fecha: 3-Mayo-2023
include 'emu8086.inc'
org 100h
MOV AX, 3
PUSH AX
MOV AX, 5
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
MOV AX, 8
PUSH AX
POP BX
POP AX
MUL  BX
PUSH AX
MOV AX, 10
PUSH AX
MOV AX, 4
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV  BX
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP AX
; Asignacion k
MOV k, AX
MOV AX, k
PUSH AX
MOV AX, 61
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE Eif1
; If: 2
; Condicion Eif1
; Printf
; Else: Eif1
; Printf
Eif1:
int 20h
RET
define_scan_num
define_print_num
define_print_num_uns
; V a r i a b l e s
altura dw 0h
i dw 0h
j dw 0h
k dw 0h
END
