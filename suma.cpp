#include <stdio.h>
#include <math.h>
#include <iostream>

float altura, i, j, k;
int a, b;

void main() // Funcion principal
{
    a = 258;
    a = (char)(a);
    a += 8;
    a *= 10;
    a /= 100;
    printf("Valor Casteado de a: ", a);
    printf("\nDigite el valor de altura: ");
    scanf("&i", &altura);

    printf("\nfor:\n");
    for (i = 1; i <= altura; i++)
    {
        printf("\t");
        for (j = 250; j < 250 + i; j++)
        {
            if (j % 2 == 0)
                printf("-");
            else
                printf("+");
        }
        printf("\n");
    }
    printf("\nwhile:\n");
    i = 1;
    while (i <= altura)
    {
        printf("\t");
        j = 250;
        while (j < 250 + i)
        {
            if (j % 2 == 0)
                printf("-");
            else
                printf("+");
            j++;
        }
        i++;
        printf("\n");
    }
    printf("\ndo:\n");
    i = 1;
    do
    {
        printf("\t");
        j = 250;
        do
        {
            if (j % 2 == 0)
                printf("-");
            else
                printf("+");
            j++;
        } while (j < 250 + i);
        i++;
        printf("\n");
    } while (i <= altura);
}