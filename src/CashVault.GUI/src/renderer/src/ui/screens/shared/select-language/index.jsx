/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import IconRadioGroup from '@ui/components/icon-radio-group'
import IconEnglishFlag from '@ui/components/icons/IconEnglishFlag'
import IconSerbianFlag from '@ui/components/icons/IconSerbianFlag'
import IconGlobe from '@ui/components/icons/IconGlobe'
import styled from '@emotion/styled'
import { useLocalizationActions, useCurrentLanguage } from '@domain/administration/stores'
import { useTranslation } from '@domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;

  .title {
    font-size: 3.438rem;
    line-height: 3.75rem;
    font-weight: 700;
    color: var(--text-light);
    max-width: 30rem;
    letter-spacing: -4%;
    margin-top: 1rem;
    min-height: 8rem;
  }
`

const OPTIONS = [
  {
    value: 'us',
    label: 'ENGLISH',
    icon: <IconEnglishFlag />
  },
  {
    value: 'bs',
    label: 'SERBIAN',
    icon: <IconSerbianFlag />
  }
]

export const SelectLanguageScreen = () => {
  const { t } = useTranslation()
  const language = useCurrentLanguage()
  const { setCurrentLanguage } = useLocalizationActions()

  const handleOnLaguageChange = (lang) => {
    if (language === lang) return

    setCurrentLanguage(lang)
  }
  return (
    <ScreenContainer hasLanguageSwitcher={false} isBottomWave={true} hasExperienceSelector={false}>
      <Container>
        <IconGlobe size="xl" color="white" />
        <p className="title">{t('Please choose your language')}</p>
      </Container>
      <IconRadioGroup options={OPTIONS} value={language} onChange={handleOnLaguageChange} />
    </ScreenContainer>
  )
}

export default SelectLanguageScreen
