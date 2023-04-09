# BME_system_design


## Project Demonstartion Video Link

[https://www.youtube.com/watch?v=5sDXhGkFigs](https://www.youtube.com/watch?v=5sDXhGkFigs)


### Introduction

This session is a description for my biomedical design project subject (Sep.2021 ~ Dec.2021)
The topic of the project is "Monitoring muscles during anaerobic exercising EMG signals + FLEX sensors".

### Topic & Objective

Recently, as people's interest in health has increased, more and more people are starting to work out. However, the proportion of people who exercise for the first time is considerable, and they are relatively unaware of their posture. Taking this wrong posture hinders proper muscle growth. Therefore, we want to create a system that warns you when you take the wrong posture. To this end, we are going to measure the EMG of each muscle and implement it in a way that warns against irritation to strange muscles.

### Background

Before proceeding with this project, we found the electrode location suitable for each muscle. First of all, if you exercise properly, ‘bench press’ stimulates the ‘pectoralis major’. And ‘Side lateral raise’ stimulates the ‘Anterior Deltoid’ when exercising properly.

### Circuit Diagram (HW Connection)

![Untitled](https://raw.githubusercontent.com/lotanda17/Images/main/BME_system_design/Image_Circuit.png)


***Operating Frequency: 48.2-159Hz***

HPF = 1/(2πRC), R=33k**Ω**, C=100nF 48.2Hz
LPF = 1/(2πRC), R=10k**Ω**, C=100nF 159Hz

### Flow Chart

![Untitled](https://raw.githubusercontent.com/lotanda17/Images/main/BME_system_design/FlowChart.png)
