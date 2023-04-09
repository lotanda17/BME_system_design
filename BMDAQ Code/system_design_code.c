#include <msp430x16x.h>


#define flexon              0xaa
#define header            0x81


int adc1,adc2,adc3;
unsigned char EMGdata[7];

void EMGinput(void);

void main(void)
{
  
        unsigned int i;
        // *****Watchdog Timer*****
         WDTCTL = WDTPW + WDTHOLD;

        // *****Basic Clock*****
        BCSCTL1 &= ~XT2OFF;
        do {
          IFG1 &= ~OFIFG;
          for (i = 0; i < 0xFF; i++);
        } while ((IFG1 & OFIFG));

        // *****Main Clock, Sub Clock*****
        BCSCTL2 = SELM_2;                     // 
        BCSCTL2 |= SELS;                       // i don't know that reason why we using fast clock. because, bench press is so slow exercising

        // *****Port Setting*****
        P1SEL &= ~(BIT6);
        P1DIR |= (BIT6);
        
        P3SEL |= BIT4 | BIT5;                    //Port 3.4, 3.5 is communication
        P6SEL= 0x07;                            //Port 6.0, 6.1, 6.2 is ADC  // 6.3 is GPIO
        P6DIR = 0x0d;                             //Port 6.1 is input of flex sensor,  6.0,6.2,6.3 is output 
       
        // *****Timer Setting*****
        
        // Timer A: SMCLK, UP mode
        TACTL = TASSEL_2 + MC_1;
        TACCTL0 = CCIE;                        // Timer A: Compare 0 Interrupt Enable
        TACCR0 = 24000;                        // fs = 250Hz
        
        // *****UART Setting*****               8.2
        ME1 |= UTXE0 + URXE0;                // Transmitter, Receiver
        UCTL0 |= CHAR;                           // 8 Bit
        UTCTL0 |= SSEL0 | SSEL1;
        UBR00 = 0x34;                              // 설정값
        UBR10 = 0x00;                              // 설정값
        UMCTL0 = 0x00;                            // 설정값
        UCTL0 &= ~SWRST;                      // 설정값
       
        
        //Set 12bit internal ADC
        ADC12CTL0 = ADC12ON | REFON | REF2_5V;
        ADC12CTL0 |= MSC;
        ADC12CTL1 = ADC12SSEL_3 | ADC12DIV_7 | CONSEQ_1;
        ADC12CTL1 |= SHP;
        
        ADC12MCTL0 = SREF_0 | INCH_0;
        ADC12MCTL1 = SREF_0 | INCH_1;
        ADC12MCTL2 = SREF_0 | INCH_2 | EOS;;                   //추가함
        ADC12CTL0 |= ENC;
              
        _BIS_SR(LPM0_bits + GIE);

        _EINT();                                        // Enable General Interrupt ************

        while(1)
        {
        }
}


#pragma vector = TIMERA0_VECTOR
__interrupt void TimerA0_interrupt()
{
        
        EMGinput();
        
        EMGdata[0] = (unsigned char) header;
        //__no_operation();
        
        EMGdata[1] = (unsigned char)(adc1>>7)&0x7F;
        EMGdata[2] = (unsigned char) adc1&0x7F;
        EMGdata[3] = (unsigned char)(adc2>>7)&0x7F;
        EMGdata[4] = (unsigned char) adc2&0x7F;
        EMGdata[5] = (unsigned char)(adc3>>7)&0x7F;
        EMGdata[6] = (unsigned char) adc3&0x7F;
        
        for(int j=0;j<7;j++){
          while (!(IFG1&UTXIFG0));
          TXBUF0=EMGdata[j];
        }
}

 void EMGinput(void)
 {
        adc1 = (int)((long)ADC12MEM0 * 9000/ 4096)-4500+7000;
        adc2 = (int)((long)ADC12MEM1 * 9000/ 4096)-4500+7000; // flex sensor
        adc3 = (int)((long)ADC12MEM2 * 9000/ 4096)-4500+7000;
      
        ADC12CTL0|=ADC12SC;
 }