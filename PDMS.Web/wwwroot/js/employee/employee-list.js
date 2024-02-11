const initialList = Array(10).fill(undefined).map((item, index) => ({
    empId: index + 1,
    empCode: 'RS28Sarmat',
    empName: 'РС-28 Сармат',
    status: true,
    email: 'src@makeyev.ru',
    phone: '+73513286333',
    position: 'Guard',
    department: 'Krasnoyarsk',
    entranceDate: new Date(2023, 9, 6),
    exitDate: null,
    address: '456320, Chelyabinsk region, Miass, Turgoyakskoe highway, 1, Russia',
    gender: true,
    avatar: 'https://media-cdn-v2.laodong.vn/storage/newsportal/2017/12/15/581542/Nga1.jpg',
    createdById: 16,
    createDate: new Date(2023, 8, 31)
}))
const options = {
    valueNames: [
        'empName',
        'empCode',
        'email',
        'position',
        'department',
        {
            data: ['empId'],
        }
    ],
    page: 10
}
const employeeList = new List('employeeList', options)


employeeList.clear()
employeeList.add(initialList)