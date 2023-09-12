#include<stdio.h>
#include<math.h>
#include<iostream>

float a,b,c,d;

void main() // Funcion principal
{
    a=(3+5)*8-(10-4)/2;
    b=8;

    printf("Valor de c = ");
    scanf("%f", &c);
    if (c % 2 == 0)
    {
        printf("\nc es par\t\tITQ");
        printf("1");
        if (c == 10)
        {
            printf("El valor de c = ",c);
            printf("2");
        }
    }
    else
    {
        printf("\nc es impar\t\tITQ");
    }

    b++;
    c--;
    c+=(10-b);
    b-=9;

    printf("El valor de a = ", a);
    printf("El valor de b = ", b);
    printf("El valor de d = ", d);
}