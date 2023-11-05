; Autor: Guillermo Fernandez Romero
; Fecha: 3-Mayo-2023
include 'emu8086.inc'
org 100h
MOV AX, 1
PUSH AX
POP AX
; Asignacion a
MOV a, AX

; Do While: 0
InicioDo0:

; Printf
print "    Hola, a = "
call print_num
MOV AX, a
PUSH AX
MOV AX, 1
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
POP AX
; Asignacion a
MOV a, AX

; Printf
print "    Hola, a = "

; Printf
print "    Hola, a = "
FinDo0:

int 20h
RET
define_scan_num
define_print_num
define_print_num_uns
; V a r i a b l e s
k dw 0h
a dw 0h
END
