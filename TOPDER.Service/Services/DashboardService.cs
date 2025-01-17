﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Dashboard;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IFeedbackRepository _feedbackRepository;


        public DashboardService(IOrderRepository orderRepository,
                                ICustomerRepository customerRepository,
                                IRestaurantRepository restaurantRepository,
                                IFeedbackRepository feedbackRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _restaurantRepository = restaurantRepository;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<DashboardAdminDTO> GetDashboardAdminAsync()
        {
            var (orders, customers, restaurants, feedbacks) = await FetchDataAsync();

            var orderStatus = await GetOrderStatusAsync(orders);

            var taskBar = await GetTaskBarDataAsync(orders, customers, restaurants);

            var topRestaurants = await GetTopRestaurantsAsync(restaurants);

            var customerAgeGroup = await GetCustomerAgeGroupAsync(customers);

            return new DashboardAdminDTO
            {
                OrderStatus = orderStatus,
                TaskBar = taskBar,
                TopRestaurantDTOs = topRestaurants,
                CustomerAgeGroup = customerAgeGroup,
                ChartCategoryRestaurants = await GetChartCategoryRestaurantsAsync(),
                MarketOverview = await GetMarketOverviewAdminAsync(orders, 0),
                YearsContainOrders = await GetYearsWithOrdersAsync(null),
            };
        }

        private async Task<List<ChartCategoryRestaurantDTO>> GetChartCategoryRestaurantsAsync()
        {
            var restaurants = await _restaurantRepository.QueryableAsync();

            var categoryRestaurantData = await restaurants
                .Include(r => r.CategoryRestaurant) 
                .Where(r => r.CategoryRestaurant != null)  
                .GroupBy(r => new { r.CategoryRestaurantId, r.CategoryRestaurant.CategoryRestaurantName })  
                .Select(g => new ChartCategoryRestaurantDTO
                {
                    CategoryId = g.Key.CategoryRestaurantId ?? 0, 
                    CategoryName = g.Key.CategoryRestaurantName ?? Is_Null.ISNULL,
                    RestaurantCount = g.Count() 
                })
                .ToListAsync();

            return categoryRestaurantData;
        }


        public async Task<DashboardRestaurantDto> GetDashboardRestaurantAsync(int restaurantId)
        {
            var query = await _restaurantRepository.QueryableAsync();

            var restaurant = await query.Include(x => x.Feedbacks).FirstOrDefaultAsync(x => x.Uid == restaurantId);

            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Restaurant with ID {restaurantId} not found.");
            }

            var feedbacks = restaurant.Feedbacks;
            var averageStar = feedbacks != null && feedbacks.Any()
                ? (int)Math.Round(feedbacks.Average(f => f.Star ?? 0))
                : 0;
            
            var dashboardDto = new DashboardRestaurantDto
            {
                Star = averageStar,
                OrderStatus = await GetOrderStatusAsync(restaurantId),
                LoyalCustomers = await GetLoyalCustomersAsync(restaurantId),
                CustomerAgeGroup = await GetCustomerAgeGroupAsync(restaurantId),
                FeedbackStars = await GetFeedbackStarsAsync(restaurantId),
                MarketOverview = await GetMarketOverviewRestaurantAsync(restaurantId,0),
                YearsContainOrders = await GetYearsWithOrdersAsync(restaurantId),
            };

            return dashboardDto;
        }
        public async Task<List<int>> GetYearsWithOrdersAsync(int? restaurantId)
        {
            var query = await _orderRepository.QueryableAsync();

            var yearsQuery = query
                .Where(o => o.CreatedAt.HasValue);

            if (restaurantId.HasValue && restaurantId != null)
            {
                yearsQuery = yearsQuery.Where(o => o.RestaurantId == restaurantId);
            }

            var years = await yearsQuery
                .Select(o => o.CreatedAt.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();

            return years;
        }

        // DATA 
        public async Task<TaskBarMonthRestaurantDTO> GetTaskBarMonthDataAsync(int restaurantId, DateTime? searchMonth)
        {
            var actualSearchMonth = searchMonth ?? DateTime.Now;

            var queryableOrders = await _orderRepository.QueryableAsync();

            var orders = queryableOrders.Where(o => o.RestaurantId == restaurantId);

            var currentMonthIncome = (double)(await orders
                .Where(o => o.CompletedAt.HasValue && o.CompletedAt.Value.Month == actualSearchMonth.Month && o.CompletedAt.Value.Year == actualSearchMonth.Year)
                .SumAsync(o => o.TotalAmount));

            var currentMonthOrdersCount = await orders
                .CountAsync(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Month == actualSearchMonth.Month && o.CreatedAt.Value.Year == actualSearchMonth.Year);

            return new TaskBarMonthRestaurantDTO
            {
                CurrentMonthIncome = new CurrentMonthIncomeDTO
                {
                    CurrentMonthIncome = currentMonthIncome,
                    IncomeGrowthRate = CalculateGrowthRate(orders, "Income", actualSearchMonth)
                },
                CurrentMonthOrder = new CurrentMonthOrderDTO
                {
                    CurrentMonthOrder = currentMonthOrdersCount,
                    OrderGrowthRate = CalculateGrowthRate(orders, "Order", actualSearchMonth)
                }
            };
        }

        public async Task<TaskBarDayRestaurantDTO> GetTaskBarDayDataAsync(int restaurantId, DateTime? searchDay)
        {
            var actualSearchDay = searchDay ?? DateTime.Now;

            var queryableOrders = await _orderRepository.QueryableAsync();

            var orders = queryableOrders.Where(o => o.RestaurantId == restaurantId);


            // Thu nhập và số lượng đơn hàng của tháng hiện tại
            var totalIncome = (double)(await orders
                .Where(o => o.CompletedAt.HasValue && o.CompletedAt.Value.Month == actualSearchDay.Month && o.CompletedAt.Value.Year == actualSearchDay.Year && o.CompletedAt.Value.Day == actualSearchDay.Day)
                .SumAsync(o => o.TotalAmount));

            var totalOrders = await orders
                .CountAsync(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Month == actualSearchDay.Month && o.CreatedAt.Value.Year == actualSearchDay.Year && o.CreatedAt.Value.Day == actualSearchDay.Day);

            return new TaskBarDayRestaurantDTO
            {
                DayOrders = totalOrders,
                DayIncome = (double)totalIncome,
            };
        }


        private async Task<OrderStatusDTO> GetOrderStatusAsync(int restaurantId)
        {
            var queryableOrders = await _orderRepository.QueryableAsync(); // Awaiting here
            var orders = await queryableOrders
                .Where(o => o.RestaurantId == restaurantId) // Now we can use Where
                .ToListAsync();

            var orderStatusData = orders
                .GroupBy(o => o.StatusOrder)
                .Select(g => new
                {
                    StatusOrder = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var totalOrders = orderStatusData.Sum(g => g.Count);

            return new OrderStatusDTO
            {
                TotalOrder = totalOrders,
                Pending = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.PENDING)?.Count ?? 0,
                Confirm = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.CONFIRM)?.Count ?? 0,
                Paid = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.PAID)?.Count ?? 0,
                Complete = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.COMPLETE)?.Count ?? 0,
                Cancel = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.CANCEL)?.Count ?? 0
            };
        }

        private async Task<List<TopLoyalCustomerDTO>> GetLoyalCustomersAsync(int restaurantId)
        {
            var queryableOrders = await _orderRepository.QueryableAsync();

            var loyalCustomers = await queryableOrders
                .Where(o => o.RestaurantId == restaurantId)
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalOrder = g.Count(),
                    TotalIncome = g.Sum(o => o.TotalAmount), 
                    Customer = g.Select(o => o.Customer).FirstOrDefault()
                })
                .OrderByDescending(c => c.TotalOrder)
                .Take(4)
                .ToListAsync();

            return loyalCustomers.Select(c => new TopLoyalCustomerDTO
            {
                CustomerId = c.CustomerId ?? 0,
                Name = c.Customer?.Name ?? string.Empty,
                Image = c.Customer?.Image ?? string.Empty,
                TotalOrder = c.TotalOrder,
                TotalInCome = (double)c.TotalIncome 
            }).ToList();
        }


        public async Task<MarketOverviewDTO> GetMarketOverviewRestaurantAsync(int restaurantId, int? filteredYear)
        {
            // Fetch orders
            var orders = await _orderRepository.QueryableAsync();

            // Determine the year to filter
            int yearToFilter = (filteredYear == null || filteredYear == 0) ? DateTime.Now.Year : filteredYear.Value;

            // Filter orders by restaurant ID and the specified year
            var filteredOrdersForYear = orders.Where(o => o.RestaurantId == restaurantId
                && o.CreatedAt.HasValue && o.CreatedAt.Value.Year == yearToFilter);

            var filteredIncomeForYear = orders.Where(o => o.RestaurantId == restaurantId
                && o.CompletedAt.HasValue && o.CompletedAt.Value.Year == yearToFilter);

            // Monthly order data
            var monthlyOrderData = filteredOrdersForYear
                .GroupBy(o => o.CreatedAt.Value.Month)
                .Select(g => new { Month = g.Key, TotalOrders = g.Count() })
                .ToList();

            // Monthly income data
            var monthlyIncomeData = filteredIncomeForYear
                .GroupBy(o => o.CompletedAt.Value.Month)
                .Select(g => new { Month = g.Key, TotalInComes = g.Sum(o => (decimal?)o.TotalAmount) }) // Nullable sum to avoid exceptions
                .ToList();

            // Create a list of months from 1 to 12
            var months = Enumerable.Range(1, 12).ToList();

            // Merge the two datasets by month, ensuring every month from 1 to 12 is included
            var mergedMonthlyData = (from month in months
                                     join order in monthlyOrderData on month equals order.Month into orderGroup
                                     from order in orderGroup.DefaultIfEmpty()
                                     join income in monthlyIncomeData on month equals income.Month into incomeGroup
                                     from income in incomeGroup.DefaultIfEmpty()
                                     select new ChartDTO
                                     {
                                         Month = month,
                                         TotalOrders = order?.TotalOrders ?? 0,  // Use 0 if no orders for the month
                                         TotalInComes = income?.TotalInComes ?? 0  // Use 0 if no income for the month
                                     }).ToList();

            var orderGrowthRateForYear = CalculateGrowthRateMarketOverview(filteredOrdersForYear, "Order", yearToFilter); // Only use filtered orders
            var incomeGrowthRateForYear = CalculateGrowthRateMarketOverview(filteredIncomeForYear, "Income", yearToFilter); // Only use filtered income


            // Calculate total income for the year, ensuring there are incomes
            double totalIncomeForYear = filteredIncomeForYear.Any() ? (double)filteredIncomeForYear.Sum(o => o.TotalAmount) : 0;

            // Return combined MarketOverviewDTO
            return new MarketOverviewDTO
            {
                TotalInComeForYear = totalIncomeForYear,
                TotalInComeGrowthRateForYear = incomeGrowthRateForYear,
                OrderForYear = filteredOrdersForYear.Count(),
                OrderGrowthRateForYear = orderGrowthRateForYear,
                MonthlyData = mergedMonthlyData
            };
        }




        private async Task<CustomerAgeGroupDTO> GetCustomerAgeGroupAsync(int restaurantId)
        {
            var currentDate = DateTime.Now;

            var ageGroupCounts = new CustomerAgeGroupDTO();

            var customers = await _customerRepository.QueryableAsync();

            var filteredCustomers = customers
                .Where(c => c.Orders.Any(o => o.RestaurantId == restaurantId) && c.Dob.HasValue);

            await filteredCustomers.ForEachAsync(c =>
            {
                var age = currentDate.Year - c.Dob.Value.Year;
                if (c.Dob.Value.Date > currentDate.AddYears(-age)) age--;

                if (age < 18) ageGroupCounts.Under18++;
                else if (age <= 24) ageGroupCounts.Between18And24++;
                else if (age <= 34) ageGroupCounts.Between25And34++;
                else if (age <= 44) ageGroupCounts.Between35And44++;
                else if (age <= 54) ageGroupCounts.Between45And54++;
                else ageGroupCounts.Above55++;
            });

            return ageGroupCounts;
        }



        private async Task<FeedbackStarDTO> GetFeedbackStarsAsync(int restaurantId)
        {
            var feedbacks = await _feedbackRepository.QueryableAsync(); // Awaiting here
            var filteredFeedbacks = await feedbacks
                .Where(f => f.RestaurantId == restaurantId)
                .ToListAsync();

            var feedbackStarData = filteredFeedbacks.GroupBy(f => f.Star)
                .Select(g => new
                {
                    Rating = g.Key,
                    Count = g.Count()
                }).ToList();

            return new FeedbackStarDTO
            {
                Star1 = feedbackStarData.FirstOrDefault(g => g.Rating == 1)?.Count ?? 0,
                Star2 = feedbackStarData.FirstOrDefault(g => g.Rating == 2)?.Count ?? 0,
                Star3 = feedbackStarData.FirstOrDefault(g => g.Rating == 3)?.Count ?? 0,
                Star4 = feedbackStarData.FirstOrDefault(g => g.Rating == 4)?.Count ?? 0,
                Star5 = feedbackStarData.FirstOrDefault(g => g.Rating == 5)?.Count ?? 0
            };
        }




        private async Task<(IQueryable<Order>, IQueryable<Customer>, IQueryable<Restaurant>, IQueryable<Feedback>)> FetchDataAsync()
        {
            var orderTask = _orderRepository.QueryableAsync();
            var customerTask = _customerRepository.QueryableAsync();
            var restaurantTask = _restaurantRepository.QueryableAsync();
            var feedbackTask = _feedbackRepository.QueryableAsync();

            await Task.WhenAll(orderTask, customerTask, restaurantTask, feedbackTask);

            return (await orderTask, await customerTask, await restaurantTask, await feedbackTask);
        }

        // Get order status
        private async Task<OrderStatusDTO> GetOrderStatusAsync(IQueryable<Order> orders)
        {
            var orderStatusData = await orders
                .GroupBy(o => o.StatusOrder)
                .Select(g => new
                {
                    StatusOrder = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var totalOrders = orderStatusData.Sum(g => g.Count);

            return new OrderStatusDTO
            {
                TotalOrder = totalOrders,
                Pending = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.PENDING)?.Count ?? 0,
                Confirm = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.CONFIRM)?.Count ?? 0,
                Paid = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.PAID)?.Count ?? 0,
                Complete = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.COMPLETE)?.Count ?? 0,
                Cancel = orderStatusData.FirstOrDefault(g => g.StatusOrder == Order_Status.CANCEL)?.Count ?? 0
            };
        }

        // Get task bar data
        private async Task<TaskBarAdminDTO> GetTaskBarDataAsync(IQueryable<Order> orders, IQueryable<Customer> customers, IQueryable<Restaurant> restaurants)
        {
            var totalOrders = await orders.CountAsync();
            var totalCustomers = await customers.CountAsync();
            var totalRestaurants = await restaurants.CountAsync();
            var totalIncome = (double)await orders.SumAsync(o => o.TotalAmount);

            var currentMonthIncome = (double)(await orders.Where(o => o.CompletedAt.HasValue && o.CompletedAt.Value.Month == DateTime.Now.Month).SumAsync(o => o.TotalAmount));
            var currentMonthOrdersCount = await orders.CountAsync(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Month == DateTime.Now.Month);

            return new TaskBarAdminDTO
            {
                TotalOrder = totalOrders,
                TotalCustomer = totalCustomers,
                TotalRestaurant = totalRestaurants,
                TotalIncome = totalIncome,
                CurrentMonthIncome = new CurrentMonthIncomeDTO
                {
                    CurrentMonthIncome = currentMonthIncome,
                    IncomeGrowthRate = CalculateGrowthRate(orders, "Income", DateTime.Now)
                },
                CurrentMonthOrder = new CurrentMonthOrderDTO
                {
                    CurrentMonthOrder = currentMonthOrdersCount,
                    OrderGrowthRate = CalculateGrowthRate(orders, "Order", DateTime.Now)
                }
            };
        }

        // Get top restaurants
        private async Task<List<TopRestaurantDTO>> GetTopRestaurantsAsync(IQueryable<Restaurant> restaurants)
        {
            return await restaurants
                .OrderByDescending(r => r.Orders.Sum(o => o.TotalAmount))
                .Take(4)
                .Select(r => new TopRestaurantDTO
                {
                    RestaurantId = r.Uid,
                    Name = r.NameRes,
                    Image = r.Logo ?? Is_Null.ISNULL,
                    TotalOrder = r.Orders.Count(),
                    TotalInCome = (double)r.Orders.Sum(o => o.TotalAmount)
                })
                .ToListAsync();
        }

        // Get customer age group distribution
        private async Task<CustomerAgeGroupDTO> GetCustomerAgeGroupAsync(IQueryable<Customer> customers)
        {
            var currentDate = DateTime.Now;

            var customerAgeGroups = await customers
                .Where(c => c.Dob.HasValue)
                .Select(c => new
                {
                    Age = currentDate.Year - c.Dob.Value.Year -
                          (c.Dob.Value.Date > currentDate.AddYears(-(currentDate.Year - c.Dob.Value.Year)).Date ? 1 : 0)
                })
                .ToListAsync();

            return new CustomerAgeGroupDTO
            {
                Under18 = customerAgeGroups.Count(c => c.Age < 18),
                Between18And24 = customerAgeGroups.Count(c => c.Age >= 18 && c.Age <= 24),
                Between25And34 = customerAgeGroups.Count(c => c.Age >= 25 && c.Age <= 34),
                Between35And44 = customerAgeGroups.Count(c => c.Age >= 35 && c.Age <= 44),
                Between45And54 = customerAgeGroups.Count(c => c.Age >= 45 && c.Age <= 54),
                Above55 = customerAgeGroups.Count(c => c.Age > 55)
            };
        }

        public async Task<MarketOverviewDTO> GetMarketOverviewAdminAsync(IQueryable<Order> orders, int? filteredYear)
        {
            // Determine the year to filter
            int yearToFilter = (filteredYear == null || filteredYear == 0) ? DateTime.Now.Year : filteredYear.Value;

            var filteredOrders = orders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Year == yearToFilter);
            var filteredIncome = orders.Where(o => o.CompletedAt.HasValue && o.CompletedAt.Value.Year == yearToFilter);

            // Get monthly order data
            var monthlyOrderData = await filteredOrders
                .GroupBy(o => o.CreatedAt.Value.Month)
                .Select(g => new { Month = g.Key, TotalOrders = g.Count() })
                .ToListAsync();

            // Get monthly income data
            var monthlyIncomeData = await filteredIncome
                .GroupBy(o => o.CompletedAt.Value.Month)
                .Select(g => new { Month = g.Key, TotalInComes = g.Sum(o => (decimal?)o.TotalAmount) }) // Nullable sum to avoid exceptions
                .ToListAsync();

            // Create a list of months from 1 to 12
            var months = Enumerable.Range(1, 12).ToList();

            // Merge the two datasets by month, ensuring every month from 1 to 12 is included
            var mergedMonthlyData = (from month in months
                                     join order in monthlyOrderData on month equals order.Month into orderGroup
                                     from order in orderGroup.DefaultIfEmpty()
                                     join income in monthlyIncomeData on month equals income.Month into incomeGroup
                                     from income in incomeGroup.DefaultIfEmpty()
                                     select new ChartDTO
                                     {
                                         Month = month,
                                         TotalOrders = order?.TotalOrders ?? 0,  // Use 0 if no orders for the month
                                         TotalInComes = income?.TotalInComes ?? 0  // Use 0 if no income for the month
                                     }).ToList();

            // Calculate growth rates
            var orderGrowthRateForYear = CalculateGrowthRateMarketOverview(filteredOrders, "Order", yearToFilter); // Only use filtered orders
            var incomeGrowthRateForYear = CalculateGrowthRateMarketOverview(filteredIncome, "Income", yearToFilter); // Only use filtered income

            // Calculate total income for the year, ensuring there are incomes
            double totalIncomeForYear = filteredIncome.Any() ? (double)filteredIncome.Sum(o => o.TotalAmount) : 0;

            // Create and return the MarketOverviewDTO
            MarketOverviewDTO marketOverviewDTO = new MarketOverviewDTO()
            {
                TotalInComeForYear = totalIncomeForYear,
                TotalInComeGrowthRateForYear = incomeGrowthRateForYear,
                OrderForYear = filteredOrders.Count(),
                OrderGrowthRateForYear = orderGrowthRateForYear,
                MonthlyData = mergedMonthlyData
            };

            return marketOverviewDTO;
        }

        private double CalculateGrowthRateMarketOverview(IQueryable<Order> orders, string type, int filteredYear)
        {
            var lastYearOrders = orders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Year == (filteredYear - 1));
            var currentYearOrders = orders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Year == filteredYear);

            if (type == "Order")
            {
                int totalOrderLastYear = lastYearOrders.Count();
                int totalCurrentYearOrders = currentYearOrders.Count();

                if (totalOrderLastYear == 0)
                    return totalCurrentYearOrders > 0 ? 100 : 0;

                return ((double)(totalCurrentYearOrders - totalOrderLastYear) / totalOrderLastYear) * 100;
            }
            else if (type == "Income")
            {
                decimal lastYearIncome = lastYearOrders.Sum(o => o.TotalAmount ?? 0);
                decimal currentYearIncome = currentYearOrders.Sum(o => o.TotalAmount ?? 0);

                if (lastYearIncome == 0)
                    return currentYearIncome > 0 ? 100 : 0;

                return ((double)(currentYearIncome - lastYearIncome) / (double)lastYearIncome) * 100;
            }

            return 0;
        }


        private double CalculateGrowthRate(IQueryable<Order> orders, string type, DateTime searchMonth)
        {
            int checkCurrentMonthOrders = searchMonth.Month;
            int checkLastMonthOrders = checkCurrentMonthOrders - 1;

            if (checkLastMonthOrders == 0)
            {
                checkLastMonthOrders = 12;
            }

            var lastMonthOrders = orders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Month == checkLastMonthOrders && o.CreatedAt.Value.Year == searchMonth.Year);
            var currentMonthOrders = orders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Month == checkCurrentMonthOrders && o.CreatedAt.Value.Year == searchMonth.Year);

            if (type == "Order")
            {
                int totalOrderLastMonth = lastMonthOrders.Count();
                int totalCurrentMonthOrders = currentMonthOrders.Count();

                if (totalOrderLastMonth == 0)
                {
                    return totalCurrentMonthOrders > 0 ? 100 : 0; 
                }

                return ((double)(totalCurrentMonthOrders - totalOrderLastMonth) / totalOrderLastMonth) * 100;
            }
            else if (type == "Income")
            {
                decimal lastMonthIncome = lastMonthOrders.Sum(o => o.TotalAmount ?? 0);
                decimal currentMonthIncome = currentMonthOrders.Sum(o => o.TotalAmount ?? 0);

                if (lastMonthIncome == 0)
                {
                    return currentMonthIncome > 0 ? 100 : 0; 
                }

                return ((double)(currentMonthIncome - lastMonthIncome) / (double)lastMonthIncome) * 100;
            }

            return 0;
        }

    }
}
