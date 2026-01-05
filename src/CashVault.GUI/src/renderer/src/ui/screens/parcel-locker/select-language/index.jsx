/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import ImageCardV2 from '@ui/components/cards/image-card-v2'
import IconGlobe from '@icons/IconGlobe'
import SRB from '@ui/assets/images/lng-serbian.png'
import ENG from '@ui/assets/images/lng-english.png'
import {
  useCurrentLanguage,
  useLocalizationActions,
  useTranslation
} from '@src/app/domain/administration/stores'

const OPTIONS = [
  { value: 'us', label: 'ENGLISH', image: <img src={ENG} /> },
  { value: 'bs', label: 'SERBIAN', image: <img src={SRB} /> }
]

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .cards {
    padding: 2rem 0 0 0;
    display: flex;
    gap: 1.5rem;
    flex-wrap: nowrap;
    overflow-x: hidden;
  }
`

const SelectLanguageScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const language = useCurrentLanguage()
  const { setCurrentLanguage } = useLocalizationActions()

  const handleLanguageChange = (lang) => {
    if (language === lang) return

    setCurrentLanguage(lang)
  }

  const actions = {
    onBack: () => navigate(-1),
    onProceed: () => navigate('/parcel-locker/select-parcel-action')
  }

  return (
    <ScreenContainerTop hasAdminButton={true} actions={actions}>
      <Container>
        <ScreenHeading
          className="language-selection-heading"
          top={() => <IconGlobe size="xl" />}
          middle={t('Please choose your language')}
        />

        <div className="cards">
          {OPTIONS.map((x, index) => (
            <ImageCardV2
              key={`parcel-locker-language-card-${index}`}
              active={language === x.value}
              onClick={() => handleLanguageChange(x.value)}
              text={t(x.label)}
              image={() => x.image}
            />
          ))}
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default SelectLanguageScreen
