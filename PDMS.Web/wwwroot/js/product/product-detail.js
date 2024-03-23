const productName = $('#productName')
const desp = $('#description')
const lastModified = $('#lastModified')
const price = $('#price')
const imgSlide = $('#img-slide')

const brandName = $('#brandName')

var galleryTop = new Swiper('.swiper-container', {
    spaceBetween: 5,
    loop: true,
    loopedSlides: 5,
    slideToClickedSlide: true,
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
});

var galleryBottom = new Swiper('.galleryThumbs', {
    spaceBetween: 5,
    slidesPerView: 5,
    loop: true,
    freeMode: true,
    hideOnClick: true,
    grabCursor: true,
    loopedSlides: 5,
    centeredSlides: true,
    slideToClickedSlide: true,
    watchSlidesVisibility: true,
    watchSlidesProgress: true,
    parent: '#galleryTop',
});

galleryTop.controller.control = galleryBottom;
galleryBottom.controller.control = galleryTop;
async function showProduct() {
    const url = window.location.href
    const productId = url
        .substring(url.lastIndexOf('/') + 1)
        .replace(/\D/g, '')

    fetchWithCredentials(`http://localhost:5000/Product/${productId}`, {
        onSuccess: async r => {
            const data = await r.json()
            productName.text(data.productName)
            $('#product-code').text(data.productCode)
            fetchWithCredentials(`http://localhost:5000/Brand/${data.brandId}`, {
                onSuccess: async i => {
                    const dataBrand = await i.json()
                    brandName.text(dataBrand.brandName)
                }
            })
            JsBarcode(document.querySelector('.barcodeImg'), data.barCode, {
                format: "CODE128",
                height: 70,
            });
            fetchWithCredentials(`http://localhost:5000/Major/${data.majorId}`, {
                onSuccess: async i => {
                    const dataMajor = await i.json()
                    $('#major-thing').text(dataMajor.majorName)
                }
            })
            fetchWithCredentials(`http://localhost:5000/Supplier/${data.suppilerId}`, {
                onSuccess: async i => {
                    const dataSuppiler = await i.json()
                    $('#supplier-thing').text(dataSuppiler.supplierName)
                }
            })



            $('#stock').text(data.quantity)
            $('#stock-can-add').attr("max", data.quantity)

            $('button[data-type="plus"]').click(function () {
                var input = $('#stock-can-add');
                var currentValue = parseInt(input.val());
                var maxValue = parseInt(input.attr('max'));
                var minValue = parseInt(input.attr('min'));
                if (currentValue >= maxValue) {
                    input.val(maxValue)
                } else if (currentValue <= minValue) {
                    input.val(minValue)
                }
            });

            $('#stock-can-add').change( () => {
                if(parseInt($('#stock-can-add').val()) > data.quantity || parseInt($('#stock-can-add').val()) === 0){
                    $('#add-to-cart').prop("disabled", true)
                }else {
                    $('#add-to-cart').prop("disabled", false)
                }
            })
            
            setTimeout(() => {
                if (user.role === "CUSTOMER") {
                    $('#add-to-cart').on('click', () => {
                        addToCart(productId, parseInt($('#stock-can-add').val()))
                    })
                } else {
                    $('#add-to-cart').prop("disabled", true)
                }
            },500)
            
            

            desp.html(data.description.replace(/(\r\n|\n|\r)/gm, '<br>'))
            lastModified.text(data.lastModifiedTime.split('T')[0])
            price.text(data.price + '₫')
            const listImg = data.image.replace(/[\[\"\]]/g, "").split(',')
            await listImg.forEach( item => {
                const divItem = document.createElement("div")
                divItem.classList.add("swiper-slide", "h-100")
                const imgElement = document.createElement("img")
                imgElement.classList.add("rounded-1", "fit-cover", "h-100", "w-100")
                imgElement.src = `http://localhost:5000/` + item
                imgElement.alt = ""
                divItem.appendChild(imgElement)

                const divItemThumbs = document.createElement("div")
                divItemThumbs.classList.add("swiper-slide", "h-100")
                const imgElementThumbs = document.createElement("img")
                imgElementThumbs.classList.add("rounded-1", "fit-cover", "h-100", "w-100")
                imgElementThumbs.src = `http://localhost:5000/` + item
                imgElementThumbs.alt = ""
                divItemThumbs.appendChild(imgElementThumbs)

                galleryTop.appendSlide(divItem)
                document.getElementById('galleryThumbs').appendChild(divItemThumbs)
            } )
        }
    })
}



showProduct()
