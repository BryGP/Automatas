#include<stdio.h>
#include<math.h>
#include<iostream>

float a,b,c,d;

void main() // Funcion principal
{
    a=(3+5)*8-(10-4)/2;
    b=19;
    printf("Valor de c = ");
    scanf("%f",&c);
    if (c%2==0)
    {

        printf("\nc es par\t\tITQ\n");
        if (c==10)
            printf("Se ejecuta el segundo If ",c);
        else
            printf("else");
            a = 70; 
    }
    else
    {
        printf("\nc es impar\tITQ\n");
        if(c==11)
            printf("Se ejecuta el segundo if del Else");
    }
    b++;
    c--;
    d = 3;
    c+=(15-b); //b = 9
    b-=9;
    printf("\nEl valor de a = ",a);
    printf("\nEl valor de b = ",b);
    printf("\nEl valor de d = ",d);
    printf("\nEl valor de c = ",c);
}