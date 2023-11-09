#include <stdio.h>
#include <math.h>

int altura,i,j;
float x,y;

void main()
{
    y = 10;
    x = 9;
    x += (3 + 5) * 8 - (10 - 4) / 2; // = 9 + 61 = 70
    x /= (y-3); // 70 / 7 = 10
    x --; // 9
    x *= (x-7); // 18
    
    printf("\nValor de altura = ");
    scanf(&altura);

    for (i = 1; i<=altura; i++)
    {
        for (j = 1; j<=i; j++)
        {
            printf(j);
        }
        printf("\n");
    }
    i = 0;
    do
    {
        printf("-");
        i++;
    }
    while (i<altura*2);
    printf("\n");
    for (i = 1; i<=altura; i++)
    {
        j = 1;
        while (j<=i)
        { 
            printf(j);
            j++;
        }
        printf("\n");
    }
    i = 0;
    do
    {
        printf("-");
        i++;
    }
    while (i<altura*2);
    printf("\n");
}