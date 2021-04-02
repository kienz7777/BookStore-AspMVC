document.addEventListener("DOMContentLoaded",function(){

    var btnDecrease = document.querySelector('.decrease');
    var btnIncrease = document.querySelector('.increase');
    var input = document.querySelector('.num')
    btnDecrease.onclick = function(){

        if(parseInt(input.value) >= 2)
        {
            input.value = parseInt(input.value) - 1;
        }
        

        console.log(input.value);
    }

    btnIncrease.onclick = function(){

        input.value = parseInt(input.value) + 1;
        console.log(input.value);
    }
},false)