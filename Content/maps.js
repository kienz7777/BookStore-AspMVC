var map;// Khởi tạo các biến global mã mình sẽ sử dụng.
var directionsDisplay;
var directionsService;
var stepDisplay;
var markerArray = [];

function initMap() {
  var myOptions = {
    zoom: 15,
    center: new google.maps.LatLng(10.849750, 106.768500),
    mapTypeId: google.maps.MapTypeId.ROADMAP
  };
  map = new google.maps.Map(document.getElementById('map'), myOptions);
  marker = new google.maps.Marker({
    map: map,
    position: new google.maps.LatLng(10.849750, 106.768500)
  });
  infowindow = new google.maps.InfoWindow({
    content: '<img src="../Assets/logo.png" alt="" style="width:90px; "><div>BookStore Companny</div>'
  });
  google.maps.event.addListener(marker, 'click', function () {
    infowindow.open(map, marker);
  });
  infowindow.open(map, marker);
  // var lat_lng = {lat: 10.849750, lng: 106.768500};  // Latitude (Kinh độ) và Longtitude (Vĩ độ) - cho biết bản đồ của bạn sẽ ở khu vực nào, khu vực mình demo là quanh Hà Nội.
  // map = new google.maps.Map(document.getElementById('map'), {  // Khởi tạo map với trong id html là map (lát nữa sẽ tạo <div id="map">)  
  //   zoom: 16,    // tỉ lệ phóng bản đồ
  //   center: lat_lng    
  // });

  directionsService = new google.maps.DirectionsService();  // Khởi tạo DirectionsService - thằng này có nhiệm vụ tính toán chỉ đường cho chúng ta.   
  directionsDisplay = new google.maps.DirectionsRenderer({ map: map });// Khởi tạo DirectionsRenderer - thằng này có nhiệm vụ hiển thị chỉ đường trên bản đồ sau khi đã tính toán.

  var onChangeHandler = function () {
    calculateAndDisplayRoute(directionsService, directionsDisplay);// Hàm xử lý và hiển thị kết quả chỉ đường    
  };
  document.getElementById('showmap').addEventListener('click', onChangeHandler); // Tạo sự kiện khi chọn điểm xuất phát   



}

function calculateAndDisplayRoute(directionsService, directionsDisplay) {
  directionsService.route({    // hàm route của DirectionsService sẽ thực hiện tính toán với các tham số truyền vào
    origin: document.getElementById('source').value,     // điểm xuất phát
    destination: document.getElementById('destination').value,   // điểm đích 
    travelMode: document.getElementById('mode').value,    // phương tiện di chuyển 
  }, function (response, status) {    // response trả về bao gồm tất cả các thông tin về chỉ đường
    if (status === google.maps.DirectionsStatus.OK) {
      directionsDisplay.setDirections(response);// hiển thị chỉ đường trên bản đồ (nét màu đỏ từ A-B)
      var myRoute = response.routes[0].legs[0];
      var instructions = '<h3 class="distance">Quãng đường: ' + myRoute.distance.text + '</h3><br>';
      document.getElementById("instructions").innerHTML = instructions;
    } else {
      window.alert('Request for getting direction is failed due to ' + status);
    }
  });
}

