# Circle Sliders

Wpf UI circle slider controls to set numeric value. Value in textBox can be formated by property ValueStringFormat. The library contains two types: 
- **BasicCircleSlider** - Value is double type (you can bind integere type), you can set min value and max value
```
<slider:BasicCircleSlider MinValue="0" MaxValue="100" Value="{Binding DoubleValue}" ValueStringFormat="N3"/>
<slider:BasicCircleSlider MinValue="0" MaxValue="100" Value="{Binding IntValue}"/>
```
- **AxisCircleSlider** - Value is double type (you can bind integere type), you can set min value and max value
```
<slider:AxisCircleSlider MinValue="0" MaxValue="100" Value="{Binding DoubleValue}" ValueStringFormat="N2"/>
```

![Screenshot](./Screenshot/CircleSliders.PNG)
