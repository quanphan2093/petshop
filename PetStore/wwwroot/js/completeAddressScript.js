const provinceSelect = document.getElementById("provinceSelect");
const districtSelect = document.getElementById("districtSelect");
const wardSelect = document.getElementById("wardSelect");

districtSelect.disabled = true;
wardSelect.disabled = true;

// Lấy danh sách tỉnh/thành phố
fetch("https://provinces.open-api.vn/api/?depth=1")
    .then(response => response.json())
    .then(data => {
        provinceSelect.innerHTML += data.map(p => `<option value="${p.code}">${p.name}</option>`).join("");
    });

// Khi chọn tỉnh/thành phố, tải danh sách quận/huyện
provinceSelect.addEventListener("change", function () {
    const provinceCode = this.value;
    districtSelect.innerHTML = '<option value="">Chọn Quận/Huyện</option>'; // Reset
    wardSelect.innerHTML = '<option value="">Chọn Phường/Xã</option>'; // Reset
    districtSelect.disabled = true;
    wardSelect.disabled = true;

    if (provinceCode) {
        fetch(`https://provinces.open-api.vn/api/p/${provinceCode}?depth=2`)
            .then(response => response.json())
            .then(data => {
                districtSelect.innerHTML += data.districts.map(d => `<option value="${d.code}">${d.name}</option>`).join("");
                districtSelect.disabled = false;
            });
    }
});

// Khi chọn quận/huyện, tải danh sách phường/xã
districtSelect.addEventListener("change", function () {
    const districtCode = this.value;
    wardSelect.innerHTML = '<option value="">Chọn Phường/Xã</option>'; // Reset
    wardSelect.disabled = true;

    if (districtCode) {
        fetch(`https://provinces.open-api.vn/api/d/${districtCode}?depth=2`)
            .then(response => response.json())
            .then(data => {
                wardSelect.innerHTML += data.wards.map(w => `<option value="${w.code}">${w.name}</option>`).join("");
                wardSelect.disabled = false;
            });
    }
});