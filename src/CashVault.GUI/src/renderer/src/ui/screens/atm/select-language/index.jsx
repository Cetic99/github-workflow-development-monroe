import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import ImageCardV2 from '@ui/components/cards/image-card-v2'
import IconGlobe from '@icons/IconGlobe'
import { useState } from 'react'
import SRB from '@ui/assets/images/lng-serbian.png'
import ITA from '@ui/assets/images/lng-italian.png'
import ENG from '@ui/assets/images/lng-english.png'

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

  const [data, _] = useState([
    { id: 1, name: 'Serbian', image: <img src={SRB} /> },
    { id: 2, name: 'English', image: <img src={ENG} /> },
    { id: 3, name: 'Italian', image: <img src={ITA} /> }
  ])

  const [selected, setSelected] = useState(2)

  const actions = {
    onBack: () => navigate(-1),
    onProceed: () => navigate('/atm/pin-input')
  }

  return (
    <ScreenContainerTop actions={actions} infoVisible={false} supportVisible={false}>
      <Container>
        <ScreenHeading top={() => <IconGlobe size="xl" />} middle="Please choose your language" />

        <div className="cards">
          {data.map((x) => (
            <ImageCardV2
              key={x.id}
              active={selected === x.id}
              onClick={() => setSelected(x.id)}
              text={x.name}
              image={() => x.image}
            />
          ))}
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default SelectLanguageScreen
