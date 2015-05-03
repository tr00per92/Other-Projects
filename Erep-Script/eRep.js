var btn = document.createElement('button');
btn.innerHTML = 'FIGHT';
btn.style.position = 'absolute';
btn.style.top = '605px';
btn.style.left = '1075px';
btn.style.background = 'red';
btn.style.color = 'white';
btn.style.height = '50px';

document.documentElement.appendChild(btn);

btn.onclick = function () {
    var c = 28;
    var t = setInterval (function () {
        document.getElementById("fight_btn").click();
    } , 1000);
    setTimeout(function () {
        clearInterval(t);
    } , c * 1000);
};
