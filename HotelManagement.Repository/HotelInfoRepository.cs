using AutoMapper;
using HotelManagement.IRepository;
using HotelManagementData;
using HotelManagementDto.Customer;
using HotelManagementDto.HotelRoom;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;

namespace HotelManagement.Repository
{
    public class HotelInfoRepository : BaseRepository, HotelInfoIRepository
    {
        HotelManagementEntities _context;
        public HotelInfoRepository()
        {
            _context = new HotelManagementEntities();
        }


        public bool CheckLoggedInCUstomer(string userName, string password)
        {
            var result = _context.CustomerInfoes.Where(x => x.Email == userName.Trim() && x.CustomerObjectId == password.Trim()).Count();
            if (result == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckLoggedInOwner(string userName, string password)
        {
            var result = _context.HotelOwners.Where(x => x.HotelUserName == userName.Trim() && x.Password == password.Trim()).Count();
            if (result == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public string AddRoom(RoomDetailsRequest roomDetailsRequest)
        {
            try
            {
                if (roomDetailsRequest == null)
                {
                    throw new Exception("Invalid Request");
                }

                var isRoomAlreadyExist = _context.HotelRooms.Where(x => x.RoomNo.Contains(roomDetailsRequest.RoomNo)).Count();
                if(isRoomAlreadyExist > 0)
                {
                    throw new Exception("Room Number already exist");
                }
                var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RoomDetailsRequest, HotelRoom>();
                    });
                var mapper = configuration.CreateMapper();
                var user = mapper.Map<HotelRoom>(roomDetailsRequest);
                user.InsertedDate = DateTime.UtcNow;
                _context.HotelRooms.Add(user);
                _context.SaveChanges();
                return "HotelRoom created successfully";


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public RoomListDto GetAvailableRoomForOwner(AvailableRoomListRequest requestAvailableRoom)
        {

            RoomListDto roomDetails = new RoomListDto();
            var inputDateTime = DateTime.UtcNow;
            if (!String.IsNullOrEmpty(requestAvailableRoom.InputDate))
            {
                inputDateTime = DateTime.ParseExact(requestAvailableRoom.InputDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); ;
            }

            int EndIndex, StartIndex = 0;
            requestAvailableRoom.PageSize = (requestAvailableRoom.PageSize <= 0) ? 1 : requestAvailableRoom.PageSize;
            requestAvailableRoom.CurrentPage = (requestAvailableRoom.CurrentPage <= 0) ? 1 : requestAvailableRoom.CurrentPage;
            EndIndex = requestAvailableRoom.PageSize * requestAvailableRoom.CurrentPage;
            StartIndex = EndIndex - requestAvailableRoom.PageSize;
            int startIndex = (requestAvailableRoom.CurrentPage - 1) * requestAvailableRoom.PageSize;
            var hotelQuery = _context.HotelRooms.Where(x => x.RoomKey != 0);
            Dictionary<string, object> filterDictionary = new Dictionary<string, object>();


            if (requestAvailableRoom.Sort != null && !string.IsNullOrWhiteSpace(requestAvailableRoom.Sort))
            {
                switch (requestAvailableRoom.Sort)
                {
                    case "Room Number":
                        hotelQuery = requestAvailableRoom.IsAscending ? hotelQuery.OrderBy(x => x.RoomNo) : hotelQuery.OrderByDescending(x => x.RoomNo);
                        break;
                    default:
                        hotelQuery = hotelQuery.OrderBy(x => x.RoomKey);
                        break;
                }
            }
            else
            {
                hotelQuery = hotelQuery.OrderBy(x => x.RoomKey);

            }

            var availableRoomQuery = hotelQuery.Select(x => new
                RoomDetail
            {
                RoomKey = x.RoomKey,
                RoomNo = x.RoomNo,
                Type = x.Type,
                Availability = x.RoomCustomerMaps.Where(y => y.BookedTime.Year == inputDateTime.Year
                            && y.BookedTime.Month == inputDateTime.Month
                            && y.BookedTime.Day == inputDateTime.Day).Count() == 0 ? "Available" : "Booked"
            });
            roomDetails.Items = availableRoomQuery.Skip(StartIndex).Take(requestAvailableRoom.PageSize).ToList();
            roomDetails.IsAscending = requestAvailableRoom.IsAscending;
            roomDetails.Sort = requestAvailableRoom.Sort;

            var TotalRows = availableRoomQuery.Count();
            roomDetails.TotalItems = TotalRows;
            {
                int totalPages = TotalRows / requestAvailableRoom.PageSize;

                if (totalPages == 0 && TotalRows > 0)
                { totalPages = 1; }
                else if (totalPages == 0 && TotalRows == 0)
                {
                    totalPages = 0;
                }
                else if (TotalRows % requestAvailableRoom.PageSize != 0)
                    totalPages++;

                roomDetails.TotalPages = totalPages;
                roomDetails.CurrentPage = requestAvailableRoom.CurrentPage;
                roomDetails.PageSize = requestAvailableRoom.PageSize;
            }
            return roomDetails;
        }


        public BookedRoomDetail BookRoom(BookRoomRequest requestAvailableRoom)
        {
            BookedRoomDetail roomDetails = new BookedRoomDetail();
            try
            {
                var fromDate = DateTime.ParseExact(requestAvailableRoom.FromDate, "yyyy-MM-dd",
                                      System.Globalization.CultureInfo.InvariantCulture);
                var toDate = DateTime.ParseExact(requestAvailableRoom.ToDate, "yyyy-MM-dd",
                                      System.Globalization.CultureInfo.InvariantCulture);
                var dateList = GetDatesBetween(fromDate, toDate);
                var availableRoomKeys = _context.RoomCustomerMaps.Where(x => dateList.Contains(EntityFunctions.TruncateTime(x.BookedTime) ?? DateTime.Now)).Distinct().Select(x => x.RoomKey).ToList();
                var availableRoom = _context.HotelRooms.Where(x => x.Type == requestAvailableRoom.RoomType && !availableRoomKeys.Contains(x.RoomKey)).Count() != 0 ? _context.HotelRooms.Where(x => x.Type == requestAvailableRoom.RoomType && !availableRoomKeys.Contains(x.RoomKey)).FirstOrDefault() : null;
                if (availableRoom != null)
                {
                    var hotelMap = new RoomCustomerMap();
                    hotelMap.InsertedDate = DateTime.UtcNow;
                    hotelMap.RoomKey = availableRoom.RoomKey;
                    hotelMap.CustomerKey = requestAvailableRoom.CustomerKey;
                    hotelMap.BookedTime = fromDate;
                    var result = _context.RoomCustomerMaps.Add(hotelMap);
                    _context.SaveChanges();
                    if (fromDate.Date != toDate.Date)
                    {
                        var count = 1;
                        foreach (var date in dateList)
                        {
                            if (count == 1)
                            {
                                count++;
                                continue;
                            }
                            hotelMap = new RoomCustomerMap();
                            hotelMap.InsertedDate = DateTime.UtcNow;
                            hotelMap.RoomKey = availableRoom.RoomKey;
                            hotelMap.CustomerKey = requestAvailableRoom.CustomerKey;
                            hotelMap.BookedTime = date.Date;
                            _context.RoomCustomerMaps.Add(hotelMap);
                            _context.SaveChanges();
                        }
                    }
                    return new BookedRoomDetail { RoomNo = result.HotelRoom.RoomNo, Type = result.HotelRoom.Type };

                }
                else
                {
                    throw new Exception("Rooms not available for the particular date");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date.Date);
            return allDates;

        }

        public MyBookingResponse GetBookedRoomHistory(MyBookingsRequest myBookingRequest)
        {

            MyBookingResponse roomDetails = new MyBookingResponse();
            var inputDateTime = DateTime.UtcNow;
            int EndIndex, StartIndex = 0;
            myBookingRequest.PageSize = (myBookingRequest.PageSize <= 0) ? 1 : myBookingRequest.PageSize;
            myBookingRequest.CurrentPage = (myBookingRequest.CurrentPage <= 0) ? 1 : myBookingRequest.CurrentPage;
            EndIndex = myBookingRequest.PageSize * myBookingRequest.CurrentPage;
            StartIndex = EndIndex - myBookingRequest.PageSize;
            int startIndex = (myBookingRequest.CurrentPage - 1) * myBookingRequest.PageSize;
            var roomQuery = _context.RoomCustomerMaps.Where(x => x.CustomerKey == myBookingRequest.CustomerKey);
            Dictionary<string, object> filterDictionary = new Dictionary<string, object>();
            if (myBookingRequest.Sort != null && !string.IsNullOrWhiteSpace(myBookingRequest.Sort))
            {
                switch (myBookingRequest.Sort)
                {
                    case "Room Number":
                        roomQuery = myBookingRequest.IsAscending ? roomQuery.OrderBy(x => x.RoomKey) : roomQuery.OrderByDescending(x => x.RoomKey);
                        break;
                    default:
                        roomQuery = roomQuery.OrderBy(x => x.RoomKey);
                        break;
                }
            }
            else
            {
                roomQuery = roomQuery.OrderBy(x => x.RoomKey);
            }
            var bookedRoomQuery = roomQuery.Select(x => new
                MyBooking
            {
                RoomKey = x.RoomKey,
                RoomNumber = x.HotelRoom != null ? x.HotelRoom.RoomNo : "",
                RoomType = x.HotelRoom != null ? x.HotelRoom.Type : "",
                BookedDate = x.BookedTime.ToString()
            });
            roomDetails.Items = bookedRoomQuery.Skip(StartIndex).Take(myBookingRequest.PageSize).ToList();
            roomDetails.IsAscending = myBookingRequest.IsAscending;
            roomDetails.Sort = myBookingRequest.Sort;
            //foreach(var item in roomDetails.Items)
            //{
            //    item.BookedDate = (item.BookedDate == "" || item.BookedDate == null) ? "" : DateTime.ParseExact(item.BookedDate, "MMM dd yyyy hh:mmtt", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            //}
            var TotalRows = bookedRoomQuery.Count();
            roomDetails.TotalItems = TotalRows;
            {
                int totalPages = TotalRows / myBookingRequest.PageSize;

                if (totalPages == 0 && TotalRows > 0)
                { totalPages = 1; }
                else if (totalPages == 0 && TotalRows == 0)
                {
                    totalPages = 0;
                }
                else if (TotalRows % myBookingRequest.PageSize != 0)
                    totalPages++;

                roomDetails.TotalPages = totalPages;
                roomDetails.CurrentPage = myBookingRequest.CurrentPage;
                roomDetails.PageSize = myBookingRequest.PageSize;
            }
            return roomDetails;
        }

        public int CreateUser(AddCustomerRequest userDto)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddCustomerRequest, CustomerInfo>();
            });
            var mapper = configuration.CreateMapper();
            var user = mapper.Map<CustomerInfo>(userDto);
            user.InsertedDate = DateTime.Now;
            _context.CustomerInfoes.Add(user);
            _context.SaveChanges();
            return user.CustomerKey;
        }

        public CustomerDetail GetCustomerDetails(ValidateCustomerRequest request)
        {
            var result = _context.CustomerInfoes.Where(x => x.Email == request.Email.Trim()).ToList();
            if (result.Count() == 0)
                throw new Exception("New Customer");
            else
            {
                CustomerDetail customerDetail = new CustomerDetail();
                customerDetail.CustomerKey = result.FirstOrDefault().CustomerKey;
                customerDetail.CustomerName = result.FirstOrDefault().CustomerName;
                customerDetail.CustomerObjectId = result.FirstOrDefault().CustomerObjectId;
                customerDetail.Email = result.FirstOrDefault().Email;
                return customerDetail;
            }
        }

        public bool ValidateHotelOwner(ValidateOwnerRequest validateOwnerRequest)
        {
            var hotelOwner = _context.HotelOwners.Where(x => x.HotelUserName == validateOwnerRequest.Username && x.Password == validateOwnerRequest.Password).Count();
            if (hotelOwner == 0)
                return false;
            else return true;
        }
    }
}
