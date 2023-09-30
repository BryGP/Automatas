#include <stdio.h>
#include <math.h>
#include <iostream>

char a;
int b, i;
float c;

void main() // Funcion principal
{
    /*
    do
    {
        printf("Ingresa un número: ");
        scanf("%f", &c);
        a = (char)((char)(c) + (float)(b));
        printf("El resultado es: %c\n", a);
    } while (c > 0.0);
    */

    c = 20;
    printf("C = ",c);
    a = (int)((char)(c) + (float)(b));

    /*if (1 == 1)
    {
        printf("\nHola");
        if (2 == 1)
        {
            printf(" \na todos");
        }
        else
        {
            printf(" \na nadie");

            for (i=0; i<10; i++)
            {
                printf("\nHola ",i);
            }
        }
    }
    else
    {
        printf("\nmundo");
    }*/
}