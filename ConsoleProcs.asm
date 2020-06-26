;
; ConsoleProcs.asm
; 
; Copyright 2018 Ángel León <luianglenlop@gmail.com>
; 
; This program is free software; you can redistribute it and/or modify
; it under the terms of the GNU General Public License as published by
; the Free Software Foundation; either version 2 of the License, or
; (at your option) any later version.
; 
; This program is distributed in the hope that it will be useful,
; but WITHOUT ANY WARRANTY; without even the implied warranty of
; MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
; GNU General Public License for more details.
; 
; You should have received a copy of the GNU General Public License
; along with this program; if not, write to the Free Software
; Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
; MA 02110-1301, USA.
;


; Funcion en C que configura el terminal para leer un caracter
; sin tener que pulsar <enter>, necesario para que ReadKey tenga el
; mismo comportamiento que en dotnet framework
extern confTerm


SECTION .TEXT

GLOBAL writeString
GLOBAL writeInt
GLOBAL readKey
GLOBAL readChar
GLOBAL readLine
GLOBAL readInt
GLOBAL readFloat
GLOBAL finPrograma
GLOBAL finProgramaError

readChar:
    nop
ret

readFloat:
    nop
ret

readInt:
    mov r14, rax ; buffer de entrada
    mov r15, rbx ; direccion  de la variable
    xor rbx, rbx
    ;mov r10, 10
    cicloReadInt:
    call readKey
    mov r10, 10
    xor rax, rax
    mov al, byte [r14]
    cmp rax, 10
    je finReadInt
    cmp rax, 0x30
    jl finProgramaError
    cmp rax, 0x39
    jg finProgramaError
    sub rax, 0x30
    add rax, rbx
    mul r10
    mov rbx, rax
    xor rdx, rdx
    ;mov rax, r10
    ;mul r10
    mov rax, r14
    jmp cicloReadInt
    finReadInt:
    xor rdx, rdx
    mov rax, rbx
    div r10
    mov [r15], rax
    ret

writeFloat:
nop
; ToDo: terminar implementacion
ret

writeChar:
nop
; ToDo: terminar implementacion
ret

writeInt:
    mov r8, rax ; valor original
    mov r9, rax ; valor para trabajar
    cmp rax, 0
    jg writeIntPositivo
    mov r9, 0
    sub r9, rax
    mov rax, r9

    writeIntPositivo:
    xor rbx, rbx ; buffer
    mov rcx, 0 ; caracteres
    mov r11, 0 ; caracteres en el buffer
    mov r12, 0 ; numero de push
    mov r10, 10
    mov rbx, 10
    inc r11
    
    writeIntCiclo:
    shl rbx, 8
    xor rdx, rdx
    div r10
    mov bl, dl
    add bl, 0x30
    inc rcx
    inc r11
    cmp r11, 8
    jl writeIntSigIter
    call writeIntGuardarStr

    writeIntSigIter:
    cmp rax, 0
    je writeIntPreparar
    mov r9, rax
    xor rdx, rdx
    jmp writeIntCiclo
    
    writeIntGuardarStr:
    inc r12
    push rbx
    xor r11, r11
    ret

    writeIntPreparar:
    cmp r8, 0
    jge writeIntScreen
    call writeIntNegativo
    writeIntScreen:
    push rbx
    inc r12
    mov rax, 1
    mov rdi, 1
    mov rsi, rsp
    mov rdx, rcx
    syscalls
    mov rcx, r12

    writeIntPop:
    pop rax
    loop writeIntPop
ret

    writeIntNegativo:
    mov bl, 0x2d
    inc r11
    cmp r11, 8
    jl writeIntNegativoRet
    call writeIntGuardarStr
    writeIntNegativoRet:
    ret
            
; Subrutina a incluirse en el codigo ensamblador.
; Necesita:
;     cadena (invertida) esté en el stack
;     rax cantidad de caracteres a imprimir
;     rbx n cantidad de push hechos (para hacer n pop al final)

writeString:
    mov rsi, rax
    mov rdx, rbx
    push rcx
    mov rax, 1
    mov rdi, 1
    syscall
    pop rcx
    pop rdi
    cicloWriteString:
    pop rax
    loop cicloWriteString
    push rdi
ret

; procedimiento que lee un caracter
; rax debe tener la direccion del buffer de entrada
readKey:
    mov rsi, rax ; direccion del buffer de entrada
    push rsi
    mov rax, 1  ; pasar los caracteres inmediatamente al proceso
    push rax
    call confTerm
    pop rax
    pop rsi
    mov rax, 0  ; syscall 0, sys_read
    mov rdi, 0  ; filedescriptor 0, stdin
;    mov rsi, bufferIn"); // buffer donde escribir
    mov rdx, 1  ; cantidade de caracteres a leer
    syscall
    mov rax, 0
    push rax
    call confTerm
    pop rax
ret

readLine:
    mov rsi, rax ; buffer donde escribir
    mov rax, 0 ; syscall 0, sys_read
    mov rdi, 0 ; filedescriptor 0, stdin
    mov rdx, 255 ; cantidad de caracteres a leer
    syscall
ret

finPrograma:
    mov rax, 60
    mov rdi, 0
    syscall

finProgramaError:
    mov rax, 60
    mov rdi, 1
    syscall