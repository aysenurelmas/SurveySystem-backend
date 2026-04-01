using Application.Features.Surveys.Rules;
using AutoMapper;
using Azure.Core;
using Business.Abstracts;
using Business.Dtos;
using Business.Messages;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.DataAccess.Paging;
using DataAccess.Abstracts;
using Entities.Concretes;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Business.Concretes;

public class SurveyManager : ISurveyService
{
    ISurveyDal _surveyDal;
    IQuestionService _questionService;
    SurveyBusinessRules _surveyBusinessRules;
    IMapper _mapper;

    public SurveyManager(ISurveyDal surveyDal, IMapper mapper, SurveyBusinessRules surveyBusinessRules, IQuestionService questionService)
    {
        _surveyDal = surveyDal;
        _mapper = mapper;
        _surveyBusinessRules = surveyBusinessRules;
        _questionService = questionService;
    }

    public async Task<CreatedSurveyResponse> Add(CreateSurveyRequest createSurveyRequest)
    {
        Survey survey = _mapper.Map<Survey>(createSurveyRequest);
        await _surveyDal.AddAsync(survey);
        CreatedSurveyResponse response = _mapper.Map<CreatedSurveyResponse>(survey);
        return response;
    }

    public async Task<UpdatedSurveyResponse> Update(UpdateSurveyRequest updateSurveyRequest)
    {
        Survey? survey = await _surveyDal.GetAsync(predicate: s => s.Id == updateSurveyRequest.Id, include:s=>s.Include(s=>s.Question));
        await _surveyBusinessRules.SurveyShouldExistWhenSelected(survey);
        survey = _mapper.Map(updateSurveyRequest, survey);

        await _surveyDal.UpdateAsync(survey!);

        UpdatedSurveyResponse response = _mapper.Map<UpdatedSurveyResponse>(survey);
        return response;
    }

    public async Task<DeletedSurveyResponse> Delete(DeleteSurveyRequest deleteSurveyRequest)
    {
        Survey? survey = await _surveyDal.GetAsync(predicate: s => s.Id == deleteSurveyRequest.Id, include: s => s.Include(s => s.Question));
        await _surveyBusinessRules.SurveyShouldExistWhenSelected(survey);

        await _surveyDal.DeleteAsync(survey!);
        await _questionService.Delete(survey.Question.Id);
        DeletedSurveyResponse response = _mapper.Map<DeletedSurveyResponse>(survey);
        return response;
    }

    public async Task<IPaginate<GetListSurveyResponse>> GetList(PageRequest pageRequest)
    {
        IPaginate<Survey> surveys = await _surveyDal.GetListAsync (
            include: s=>s.Include(s=>s.Participations), 
            index: pageRequest.PageIndex,
            size: pageRequest.PageSize
            );

        var response = _mapper.Map<Paginate<GetListSurveyResponse>>(surveys);
        return response;
    }
    public async Task<GetByIdSurveyResponse> GetSurveyDetailsById(int surveyId)
    {
        Survey? survey = await _surveyDal.GetAsync(
            predicate: s => s.Id == surveyId,
            include: s => s.Include(s => s.Question));

        await _surveyBusinessRules.SurveyShouldExistWhenSelected(survey);

        GetByIdSurveyResponse response = _mapper.Map<GetByIdSurveyResponse>(survey);
        return response;
    }
    public async Task<SurveyResultResponse> ParticipateInSurvey(int surveyId )
    {
        Survey? survey = await _surveyDal.GetAsync(include:s=>s.Include(s=>s.Question).Include(s=>s.Participations),
            predicate: s => s.Id == surveyId);
        await _surveyBusinessRules.SurveyShouldExistWhenSelected(survey);
        var response = _mapper.Map<SurveyResultResponse>(survey);
        return response;
    }
}
