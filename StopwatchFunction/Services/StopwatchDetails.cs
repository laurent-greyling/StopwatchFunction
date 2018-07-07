using StopwatchFunction.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StopwatchFunction.Helpers;
using StopwatchFunction.Models;

namespace StopwatchFunction.Services
{
    public class StopwatchDetails : IStopwatchDetails
    {
        private readonly IAzureService _azureService;
        private readonly ElapsedTime _elapsedTime;
        public StopwatchDetails(IAzureService azureService)
        {
            _azureService = azureService;
            _elapsedTime = new ElapsedTime();
        }

        public async Task Save(StopwatchEntity stopwatchEntity)
        {
            try
            {
                await _azureService.UpdateTableAsync(stopwatchEntity);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }            
        }

        //This is for when the webjob runs, this will get elapsed time the webjob saves into table. The webjob is not efficient and accurate.
        //Leaving this in more as a reference as this is not production code just example code
        public async Task<List<UserDetailsModel>> Retrieve(UserDetailsEntity userDetailsEntity)
        {
            try
            {
                var entityList = new List<UserDetailsModel>();
                if (!string.IsNullOrEmpty(userDetailsEntity.UserName) && !string.IsNullOrEmpty(userDetailsEntity.StopWatchName))
                {                    
                    var userEntity = await _azureService.RetrieveEntityAsync(userDetailsEntity);
                    entityList.Add(new UserDetailsModel
                    {
                        UserName = ((UserDetailsEntity)userEntity.Result).UserName,
                        StopWatchName = ((UserDetailsEntity)userEntity.Result).StopWatchName,
                        Status = ((UserDetailsEntity)userEntity.Result).Status,
                        ElapsedTime = ((UserDetailsEntity)userEntity.Result).ElapsedTime
                    });
                }

                if (!string.IsNullOrEmpty(userDetailsEntity.StopWatchName)) return entityList;

                var entities = _azureService.RetrieveEntities(userDetailsEntity);
                entityList.AddRange(entities.Select(x => new UserDetailsModel
                {
                    UserName = x.UserName,
                    StopWatchName = x.StopWatchName,
                    Status = x.Status,
                    ElapsedTime = x.ElapsedTime
                }).ToList());

                return entityList;
            } 
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<UserDetailsModel>> RetrieveElaspedTime(UserDetailsEntity userDetailsEntity)
        {
            try
            {
                var entityList = new List<UserDetailsModel>();
                if (!string.IsNullOrEmpty(userDetailsEntity.UserName) && !string.IsNullOrEmpty(userDetailsEntity.StopWatchName))
                {
                    var userEntity = await _azureService.RetrieveEntityAsync(userDetailsEntity);
                    entityList.Add(new UserDetailsModel
                    {
                        UserName = ((UserDetailsEntity)userEntity.Result).UserName,
                        StopWatchName = ((UserDetailsEntity)userEntity.Result).StopWatchName,
                        Status = ((UserDetailsEntity)userEntity.Result).Status,
                        ElapsedTime = _elapsedTime.CalculateElapsedTime(((UserDetailsEntity)userEntity.Result).StartTime, ((UserDetailsEntity)userEntity.Result).EndTime)
                    });
                }

                if (!string.IsNullOrEmpty(userDetailsEntity.StopWatchName)) return entityList;

                var entities = _azureService.RetrieveEntities(userDetailsEntity);
                entityList.AddRange(entities.Select(x => new UserDetailsModel
                {
                    UserName = x.UserName,
                    StopWatchName = x.StopWatchName,
                    Status = x.Status,
                    ElapsedTime = _elapsedTime.CalculateElapsedTime(x.StartTime, x.EndTime)
                }).ToList());

                return entityList;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }

        
    }
}
